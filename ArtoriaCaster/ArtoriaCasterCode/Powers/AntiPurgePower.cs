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
    public override async Task AfterPowerAmountChanged(MegaCrit.Sts2.Core.Models.PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(power, amount, applier, cardSource);
        if (power != this || _isClamping || Amount <= Max) return;

        _isClamping = true;
        await PowerCmd.ModifyAmount(this, Max - Amount, Owner, null);
        _isClamping = false;
    }

    /// <summary>Enemy hits fully stopped this turn: Block-stopped (FormPower) + AP-only annulments.</summary>
    public static int FullyStoppedHits(Creature creature) =>
        FormPower.GetBlockedHits(creature) +
        ((creature.GetPowerInstances<AntiPurgePower>().FirstOrDefault())?.AnnulledThisTurn ?? 0);

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
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
            await NotifyAnnulled(dealer);
        }
        await PowerCmd.Decrement(this);
    }

    private async Task NotifyAnnulled(Creature attacker)
    {
        foreach (var power in Owner.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>())
        {
            if (power is IHitAnnulledListener listener) await listener.AfterHitAnnulled(attacker);
        }
        var relics = Owner.Player?.Relics;
        if (relics == null) return;
        foreach (var relic in relics)
        {
            if (relic is IHitAnnulledListener listener) await listener.AfterHitAnnulled(attacker);
        }
    }
}

/// <summary>
/// Reacts to an Anti-Purge annulment that vanilla did not count as a fully blocked
/// hit. Implementors wanting EVERY fully-stopped hit must also check
/// result.WasFullyBlocked in their own AfterDamageReceived (Ojos Feéricos pattern).
/// </summary>
public interface IHitAnnulledListener
{
    Task AfterHitAnnulled(Creature attacker);
}
