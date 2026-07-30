using AstolfoRider.AstolfoRiderCode.Caprice;
using AstolfoRider.AstolfoRiderCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Powers;

public sealed class EvaporatedReasonDPower : AstolfoPower, ICapriceFulfilledListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment f)
    {
        if (f.Owner != Owner || f.NumberThisTurn != 1) return;
        Flash();
        await NpCharge.Gain(context, Owner, 10, f.Card);
        await CritStars.Gain(context, Owner, 10, f.Card);
    }
}

public sealed class FreeCapriceCommandPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public CommandType ChosenType => Caprices.FromBit((int)Amount);

    public override bool TryModifyEnergyCostInCombat(
        CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner?.Creature != Owner || card is not AstolfoCommandCard { IsNoblePhantasm: false } command ||
            command.CommandType != ChosenType || card.Pile?.Type is not (PileType.Hand or PileType.Play))
            return false;
        modifiedCost = 0m;
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card is not AstolfoCommandCard { IsNoblePhantasm: false } command ||
            command.CommandType != ChosenType) return;
        Flash();
        await PowerCmd.Remove(this);
    }
}

public sealed class PaladinsInstinctPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Caprices.FulfilledThisTurn(Owner) > 0) return;
        await PowerCmd.Apply<PaladinsInstinctOwedPower>(
            context, Owner, Amount, Owner, null, silent: true);
    }
}

public sealed class PaladinsInstinctOwedPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        var block = Amount;
        await PowerCmd.Remove(this);
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);
    }
}

public sealed class ContagiousGoodHumorPower : AstolfoPower, ICapriceFulfilledListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment f)
    {
        if (f.Owner != Owner) return;
        Flash();
        foreach (var player in Owner.CombatState?.PlayerCreatures.Where(x => !x.IsDead).ToList() ?? [])
            await CreatureCmd.GainBlock(player, Amount, ValueProp.Unpowered, null);
        await PowerCmd.Remove(this);
    }
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class RidingAPlusPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (AstolfoTurnUsages.WasUsed(Owner, AstolfoTurnUsage.Riding) ||
            cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card is not AstolfoCommandCard { IsNoblePhantasm: false, CommandType: CommandType.Quick } command) return;
        await AstolfoTurnUsages.Mark(
            new BlockingPlayerChoiceContext(), Owner, AstolfoTurnUsage.Riding, command);
        command.ExternalDamageBonusTotal += Amount;
        Flash();
        await CritStars.Gain(new BlockingPlayerChoiceContext(), Owner, 10, command);
    }
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is AstolfoCommandCard command) command.ExternalDamageBonusTotal = 0m;
        return Task.CompletedTask;
    }
}

public sealed class IndependentActionBPower : AstolfoPower, ICriticalConsumedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (AstolfoTurnUsages.WasUsed(Owner, AstolfoTurnUsage.IndependentAction) ||
            critical.Owner != Owner) return;
        await AstolfoTurnUsages.Mark(context, Owner, AstolfoTurnUsage.IndependentAction, critical.Card);
        Flash();
        await CritStars.Gain(context, Owner, (int)Amount, critical.Card);
    }
}

public sealed class ImpossibleExistencePower : AstolfoPower, IEvasionConsumedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterEvasionConsumed(PlayerChoiceContext context, EvasionConsumed evasion)
    {
        if (AstolfoTurnUsages.WasUsed(Owner, AstolfoTurnUsage.ImpossibleExistence) ||
            evasion.Owner != Owner) return;
        await AstolfoTurnUsages.Mark(context, Owner, AstolfoTurnUsage.ImpossibleExistence, null);
        Flash();
        await CritStars.Gain(context, Owner, (int)Amount, null);
    }
}

public sealed class DifferentAdventureEveryTimePower : AstolfoPower, ICapriceDrawListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterCapriceDrawn(PlayerChoiceContext context, CapriceDraw draw)
    {
        var last = Owner.GetPower<LastCapricePower>();
        if (AstolfoTurnUsages.WasUsed(Owner, AstolfoTurnUsage.DifferentAdventure) ||
            draw.Owner != Owner || last == null || last.CapriceType == draw.Type) return;
        await AstolfoTurnUsages.Mark(context, Owner, AstolfoTurnUsage.DifferentAdventure, null);
        Flash();
        await NpCharge.Gain(context, Owner, 10, null);
        await CritStars.Gain(context, Owner, 10, null);
    }
}
