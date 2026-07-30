using AstolfoRider.AstolfoRiderCode.Caprice;
using AstolfoRider.AstolfoRiderCode.Cards.Special;
using AstolfoRider.AstolfoRiderCode.Cards.Uncommon;
using AstolfoRider.AstolfoRiderCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Relics;

public sealed class HippogriffFeather : AstolfoRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<HippogriffFeatherUsedPower>() || cardPlay.Card.Owner != Owner ||
            cardPlay.Card is not ICommandTyped { CommandType: CommandType.Quick }) return;
        await PowerCmd.Apply<HippogriffFeatherUsedPower>(
            context, Owner.Creature, 1m, Owner.Creature, cardPlay.Card, silent: true);
        Flash();
        await CritStars.Gain(context, Owner.Creature, 20, cardPlay.Card);
    }
}

public sealed class TrifasRibbon : AstolfoRelic, ICapriceFulfilledListener
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override Task BeforeCombatStartLate() =>
        CreatureCmd.GainBlock(Owner.Creature, 6m, ValueProp.Unpowered, null);
    public async Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment f)
    {
        if (f.Owner != Owner.Creature || Owner.Creature.HasPower<TrifasRibbonUsedPower>()) return;
        await PowerCmd.Apply<TrifasRibbonUsedPower>(
            context, Owner.Creature, 1m, Owner.Creature, f.Card, silent: true);
        Flash();
        await NpCharge.Gain(context, Owner.Creature, 10, f.Card);
    }
}

public sealed class GoldenArgaliaPoint : AstolfoRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0m || power is not WeakPower || applier != Owner.Creature ||
            cardSource?.Owner != Owner || Owner.Creature.HasPower<GoldenArgaliaPointUsedPower>()) return;
        await PowerCmd.Apply<GoldenArgaliaPointUsedPower>(
            context, Owner.Creature, 1m, Owner.Creature, cardSource, silent: true);
        Flash();
        await CritStars.Gain(context, Owner.Creature, 10, cardSource);
    }
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) &&
            Owner.Creature.GetPower<GoldenArgaliaPointUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class DoodledManual : AstolfoRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (amount >= 0m || cardSource?.Owner != Owner ||
            Owner.Creature.HasPower<DoodledManualUsedPower>()) return;
        var removedOwnDebuff = power.Owner == Owner.Creature &&
                               power.TypeForCurrentAmount == PowerType.Debuff && power is not IResourcePower;
        var removedEnemyBuff = power.Owner.Side != Owner.Creature.Side && Cleanse.IsOffensiveBuff(power);
        if (!removedOwnDebuff && !removedEnemyBuff) return;
        await PowerCmd.Apply<DoodledManualUsedPower>(
            context, Owner.Creature, 1m, Owner.Creature, cardSource, silent: true);
        Flash();
        await PowerCmd.Apply<ArtifactPower>(context, Owner.Creature, 1m, Owner.Creature, cardSource);
        await NpCharge.Gain(context, Owner.Creature, 20, cardSource);
    }
}

public sealed class ImpossibleExistenceScale : AstolfoRelic, IEvasionConsumedListener
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public async Task AfterEvasionConsumed(PlayerChoiceContext context, EvasionConsumed evasion)
    {
        if (evasion.Owner != Owner.Creature || Owner.Creature.HasPower<ImpossibleScaleUsedPower>()) return;
        await PowerCmd.Apply<ImpossibleScaleUsedPower>(
            context, Owner.Creature, 1m, Owner.Creature, null, silent: true);
        Flash();
        await CritStars.Gain(context, Owner.Creature, 10, null);
    }
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) &&
            Owner.Creature.GetPower<ImpossibleScaleUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class BorrowedAchillesShield : AstolfoRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner.Creature || delta >= 0m || creature.CurrentHp * 2 > creature.MaxHp ||
            creature.HasPower<BorrowedAchillesShieldUsedPower>()) return;
        var context = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<BorrowedAchillesShieldUsedPower>(
            context, creature, 1m, creature, null, silent: true);
        Flash();
        foreach (var player in creature.CombatState?.PlayerCreatures.Where(x => !x.IsDead).ToList() ?? [])
            await CreatureCmd.GainBlock(player, 8m, ValueProp.Unpowered, null);
        await Evasion.Grant(context, creature, 1, null);
    }
}

public sealed class AdventureBag : AstolfoRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override async Task BeforeCombatStartLate()
    {
        if (Owner.Creature.HasPower<AdventureBagChosenPower>() ||
            Owner.Creature.CombatState is not { } state) return;
        var context = new BlockingPlayerChoiceContext();
        var options = new List<CardModel>
        {
            state.CreateCard(ModelDb.Card<ChooseAdventureNp>(), Owner),
            state.CreateCard(ModelDb.Card<ChooseAdventureStars>(), Owner),
            state.CreateCard(ModelDb.Card<ChooseAdventureCaprice>(), Owner)
        };
        var selected = await CardSelectCmd.FromChooseACardScreen(context, options, Owner, false);
        if (selected is not IAdventureBagChoice choice) return;
        await PowerCmd.Apply<AdventureBagChosenPower>(
            context, Owner.Creature, 1m, Owner.Creature, selected, silent: true);
        Flash();
        switch (choice.Choice)
        {
            case AdventureBagChoice.Np:
                await NpCharge.Gain(context, Owner.Creature, 30, selected);
                break;
            case AdventureBagChoice.Stars:
                await CritStars.Gain(context, Owner.Creature, 30, selected);
                break;
            case AdventureBagChoice.Caprice:
                await UncommonRules.ChooseCaprice(context, selected);
                break;
        }
    }
}
