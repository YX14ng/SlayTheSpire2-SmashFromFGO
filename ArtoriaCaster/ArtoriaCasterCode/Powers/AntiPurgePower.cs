using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Anti-Purga (Anti-Purge / 対粛正防御 / 对肃正防御) — the Anti-Purge Defense of
/// Castoria's real NP, capped at <see cref="Max"/> (the verified Overcharge Count
/// 1→5). The next X enemy attacks that would reach the owner are nullified
/// COMPLETELY: damage capped to 0 BEFORE Block is consumed (Intangible pattern,
/// stronger), losing 1 stack per nullified hit.
/// Cap-to-0 quirk (decompiled CreatureCmd line 154): a 0-damage hit only counts as
/// WasFullyBlocked when the target happens to hold Block — so this power keeps its
/// own count of annulments vanilla missed and notifies <see cref="IHitAnnulledListener"/>s
/// under that same guard (no double counting with FormPower.BlockedHitsThisTurn).
/// </summary>
public sealed class AntiPurgePower : ArtoriaPower
{
    public const int Max = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>AP annulments this turn that vanilla did NOT count as fully blocked.</summary>
    public int AnnulledThisTurn { get; private set; }

    private bool _isClamping;

    /// <summary>
    /// Self-clamp to <see cref="Max"/> (5 = the verified Overcharge Count). Every application
    /// site (cards, relics, co-op grants) uses PowerCmd.Apply directly, so centralizing the cap
    /// here guarantees no chain of grants pushes the Counter past 5 (mirror of Stars.Gain's cap).
    /// </summary>
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Models.PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        if (power != this || _isClamping || Amount <= Max) return;

        _isClamping = true;
        try
        {
            await PowerCmd.ModifyAmount(choiceContext, this, Max - Amount, Owner, null);
        }
        finally
        {
            // try/finally (audit 2026-07-04): si el ModifyAmount lanza/cancela, el guard quedaba
            // en true y el tope de 5 se desactivaba el resto del combate.
            _isClamping = false;
        }
    }

    /// <summary>Enemy hits fully stopped this turn: Block-stopped (FormPower) + AP-only annulments.</summary>
    public static int FullyStoppedHits(Creature creature) =>
        FormPower.GetBlockedHits(creature) +
        ((creature.GetPowerInstances<AntiPurgePower>().FirstOrDefault())?.AnnulledThisTurn ?? 0);

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // Reset al arrancar el turno ENEMIGO (audit 2026-07-04, espejo de FormPower.BlockedHitsThisTurn):
        // las anulaciones ocurren en la volea enemiga y las cartas (CounterBlade) las leen en el turno
        // del jugador SIGUIENTE. Con el reset en el turno del jugador, leían siempre 0.
        if (side == CombatSide.Enemy)
        {
            AnnulledThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack() || Amount <= 0)
        {
            return decimal.MaxValue;
        }
        return 0m;
    }

    public override Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack() || Amount <= 0) return;

        // This hit landed while a stack was up: it was annulled by us.
        if (!result.WasFullyBlocked)
        {
            AnnulledThisTurn++;
            await NotifyAnnulled(choiceContext, dealer);
        }
        await PowerCmd.Decrement(this);
    }

    private async Task NotifyAnnulled(PlayerChoiceContext choiceContext, Creature attacker)
    {
        // Se materializa con ToList (los listeners pueden mutar powers) y se propaga el choiceContext
        // SINCRONIZADO del hook (audit 2026-07-04): el contraataque de la Guardiana dañaba con un
        // ThrowingPlayerChoiceContext fresco — la misma clase de bug que colgaba el turno del enjambre.
        foreach (var power in Owner.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>().OfType<IHitAnnulledListener>().ToList())
        {
            await power.AfterHitAnnulled(choiceContext, attacker);
        }
        var relics = Owner.Player?.Relics;
        if (relics == null) return;
        foreach (var relic in relics)
        {
            if (relic is IHitAnnulledListener listener) await listener.AfterHitAnnulled(choiceContext, attacker);
        }
    }
}

/// <summary>
/// Reacts to an Anti-Purge annulment that vanilla did not count as a fully blocked
/// hit. Implementors wanting EVERY fully-stopped hit must also check
/// result.WasFullyBlocked in their own AfterDamageReceived (Ojos Feéricos pattern).
/// El <paramref name="choiceContext"/> es el del hook de daño aguas arriba (sincronizado):
/// cualquier daño/efecto del listener DEBE usarlo, nunca un contexto fresco.
/// </summary>
public interface IHitAnnulledListener
{
    Task AfterHitAnnulled(PlayerChoiceContext choiceContext, Creature attacker);
}
