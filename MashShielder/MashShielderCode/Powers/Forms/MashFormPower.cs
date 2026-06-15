using BaseLib.Extensions;
using MashShielder.MashShielderCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>
/// Base for Mash's three forms (Shielder / Ortinax / Paladin) on top of FGOCore's
/// generic FormPower. Each passive is a flag so Paladin can combine them.
/// </summary>
public abstract class MashFormPower : FormPower
{
    public const int FirstBlockBonus = 3;
    public const int ShielderEndTurnThreshold = 8;
    public const int ShielderEndTurnCharge = 5;
    public const int BunkerBoltMax = 5;

    // Icons live in MashShielder's resources, not FGOCore's.
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    /// <summary>End your turn with Block → NP Charge; first Block card each turn is stronger.</summary>
    protected virtual bool ShielderPassive => false;

    /// <summary>Attacks consume up to 5 Block and add that much damage (Bunker Bolt).</summary>
    protected virtual bool OrtinaxPassive => false;

    /// <summary>Defense cards grant 1 less Block (Ortinax only).</summary>
    protected virtual bool DefensePenalty => false;

    private bool _blockCardBonusUsed;

    // Bunker Bolt: el bono se calcula y el Bloqueo se consume UNA sola vez por carta en
    // BeforeCardPlayed (camino REAL de juego; las previews no lo invocan). _pendingBunkerBonus
    // queda cacheado para que ModifyDamageAdditive lo devuelva en el PRIMER golpe y lo ponga a 0
    // (sin doble-dip en multi-hit). En preview _pendingBunkerBonus es 0 → early-return sin mutar,
    // así que zeroearlo en el hook de daño es seguro (anti-patrón de mutar-en-preview evitado).
    private int _pendingBunkerBonus;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (side == CombatSide.Player)
        {
            _blockCardBonusUsed = false;
        }
    }

    /// <summary>
    /// Multiplayer: each turn Mash offers to cover her allies — a free, Ethereal
    /// "Behind Me!" card. Only generated while a living allied player exists.
    /// </summary>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        var hasAlly = Owner.CombatState.PlayerCreatures.Any(c => c != Owner && !c.IsDead);
        if (!hasAlly) return;

        var card = Owner.CombatState.CreateCard<Cards.Special.BehindMeSenpai>(Owner.Player);
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 1.0f);
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner || cardSource == null) return 0m;

        var delta = 0m;
        if (ShielderPassive && !_blockCardBonusUsed) delta += FirstBlockBonus;
        if (DefensePenalty) delta -= 1m;
        return delta;
    }

    public override Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (creature == Owner && cardSource != null) _blockCardBonusUsed = true;
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (!ShielderPassive || side != CombatSide.Player || Owner.Block < ShielderEndTurnThreshold) return;
        Flash();
        await NpCharge.Gain(Owner, ShielderEndTurnCharge, null);
    }

    /// <summary>
    /// Consume hasta 5 de Bloqueo UNA vez por carta (antes de resolver los golpes) y cachea el bono.
    /// Así un Ataque multi-hit no consume 5 por golpe ni suma +5 a cada golpe (era double-dip).
    /// </summary>
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!OrtinaxPassive) return;
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner) return;

        var consume = (int)Math.Min(BunkerBoltMax, Owner.Block);
        if (consume <= 0) return;

        _pendingBunkerBonus = consume;
        Flash();
        await CreatureCmd.LoseBlock(Owner, consume);
    }

    // Lectura del bono cacheado: lo devuelve en el PRIMER golpe del Ataque y lo consume (a 0) para
    // que los golpes restantes no lo repitan. En preview _pendingBunkerBonus es 0 → no muta nada.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!OrtinaxPassive || Owner != dealer || !props.IsPoweredAttack() || _pendingBunkerBonus <= 0) return 0m;
        var bonus = _pendingBunkerBonus;
        _pendingBunkerBonus = 0;
        return bonus;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Safety net: si el Ataque no resolvió ningún golpe (fizzle), limpiar el bono cacheado.
        if (OrtinaxPassive && cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner)
        {
            _pendingBunkerBonus = 0;
        }
        return Task.CompletedTask;
    }
}
