using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>
/// Base for Mash's three forms (Shielder / Ortinax / Paladin). Only one is active at a time;
/// switching goes through <see cref="Forms"/>. Each passive is a flag so Paladin can combine them.
/// </summary>
public abstract class FormPower : MashShielderPower
{
    public const int FirstBlockBonus = 3;
    public const int ShielderEndTurnThreshold = 8;
    public const int ShielderEndTurnCharge = 5;
    public const int BunkerBoltMax = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>End your turn with Block → NP Charge; first Block card each turn is stronger.</summary>
    protected virtual bool ShielderPassive => false;

    /// <summary>Attacks consume up to 5 Block and add that much damage (Bunker Bolt).</summary>
    protected virtual bool OrtinaxPassive => false;

    /// <summary>Defense cards grant 1 less Block (Ortinax only).</summary>
    protected virtual bool DefensePenalty => false;

    private bool _blockCardBonusUsed;

    /// <summary>Enemy hits your Block fully stopped this turn (for Represalia and friends).</summary>
    public int BlockedHitsThisTurn { get; private set; }

    public static int GetBlockedHits(Creature creature) =>
        creature.GetPowerInstances<FormPower>().FirstOrDefault()?.BlockedHitsThisTurn ?? 0;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _blockCardBonusUsed = false;
            BlockedHitsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && dealer != null && props.IsPoweredAttack() && result.WasFullyBlocked)
        {
            BlockedHitsThisTurn++;
        }
        return Task.CompletedTask;
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

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!OrtinaxPassive || Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return Math.Min(BunkerBoltMax, Owner.Block);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!OrtinaxPassive) return;
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner) return;

        var consume = Math.Min(BunkerBoltMax, Owner.Block);
        if (consume > 0)
        {
            Flash();
            await CreatureCmd.LoseBlock(Owner, consume);
        }
    }
}
