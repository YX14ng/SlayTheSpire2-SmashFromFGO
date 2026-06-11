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
