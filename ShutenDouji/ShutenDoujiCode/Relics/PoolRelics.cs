using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ShutenDouji.ShutenDoujiCode.Cards.Special;
using ShutenDouji.ShutenDoujiCode.Powers;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Relics;

public sealed class MountOoeKanzashi : ShutenRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.Creature.HasPower<MountOoeKanzashiUsedPower>() || amount <= 0m ||
            power is not PoisonPower || applier != Owner.Creature || cardSource?.Owner != Owner) return;
        await PowerCmd.Apply<MountOoeKanzashiUsedPower>(context, Owner.Creature, 1m,
            Owner.Creature, cardSource, silent: true);
        Flash();
        await SakeBank.Gain(context, Owner.Creature, 20, cardSource);
        await NpCharge.Gain(context, Owner.Creature, 10, cardSource);
    }
}

public sealed class HakuBell : ShutenRelic, IStylePlayedListener
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task BeforeCombatStartLate()
    {
        await CreatureCmd.GainBlock(Owner.Creature, 6m, ValueProp.Unpowered, null);
    }

    public async Task AfterStylePlayed(PlayerChoiceContext context, StylePlay play)
    {
        if (play.Owner != Owner.Creature || play.Style != ShutenStyle.Caster ||
            play.Card is not IShutenStyleCard { IsShutenNp: false } ||
            Owner.Creature.HasPower<HakuBellUsedPower>()) return;
        await PowerCmd.Apply<HakuBellUsedPower>(context, Owner.Creature, 1m,
            Owner.Creature, play.Card, silent: true);
        Flash();
        await SakeBank.Gain(context, Owner.Creature, 10, play.Card);
    }
}

public sealed class PoisonedCup : ShutenRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterDamageGiven(
        PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (Owner.Creature.HasPower<PoisonedCupUsedPower>() || target.Side == Owner.Creature.Side ||
            !PoisonDamageRules.IsPoisonTick(dealer, result, props, target, cardSource)) return;
        await PowerCmd.Apply<PoisonedCupUsedPower>(context, Owner.Creature, 1m,
            Owner.Creature, null, silent: true);
        Flash();
        await NpCharge.Gain(context, Owner.Creature, 10, null);
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.Creature.GetPower<PoisonedCupUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class RedDragonUlna : ShutenRelic, ISakeSpentListener
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public async Task AfterSakeSpent(PlayerChoiceContext context, SakeChange change)
    {
        if (change.Owner != Owner.Creature || change.Amount < 30 ||
            Owner.Creature.HasPower<RedDragonUlnaUsedPower>()) return;
        await PowerCmd.Apply<RedDragonUlnaUsedPower>(context, Owner.Creature, 1m,
            Owner.Creature, change.Source, silent: true);
        Flash();
        await CritStars.Gain(context, Owner.Creature, 30, change.Source);
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.Creature.GetPower<RedDragonUlnaUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class OniHead : ShutenRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeCombatStartLate()
    {
        await PowerCmd.Apply<OniHeadGutsPower>(new BlockingPlayerChoiceContext(), Owner.Creature,
            1m, Owner.Creature, null);
    }
}

public sealed class KuzuryuFragment : ShutenRelic, IStyleCrossListener
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public async Task AfterStyleCrossed(PlayerChoiceContext context, StylePlay play)
    {
        if (play.Owner != Owner.Creature || Owner.Creature.HasPower<KuzuryuFragmentUsedPower>()) return;
        await PowerCmd.Apply<KuzuryuFragmentUsedPower>(context, Owner.Creature, 1m,
            Owner.Creature, play.Card, silent: true);
        await PowerCmd.Apply<KuzuryuFragmentAttackPower>(context, Owner.Creature, 2m,
            Owner.Creature, play.Card);
        Flash();
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.Creature.GetPower<KuzuryuFragmentUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class AntiquitiesTreasure : ShutenRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override async Task BeforeCombatStartLate()
    {
        if (Owner.Creature.HasPower<AntiquitiesTreasureChosenPower>() || Owner.Creature.CombatState is not { } state)
            return;
        var context = new BlockingPlayerChoiceContext();
        var options = new List<CardModel>
        {
            state.CreateCard(ModelDb.Card<ChooseAntiquityNp>(), Owner),
            state.CreateCard(ModelDb.Card<ChooseAntiquitySake>(), Owner),
            state.CreateCard(ModelDb.Card<ChooseAntiquityStars>(), Owner)
        };
        var selected = await CardSelectCmd.FromChooseACardScreen(context, options, Owner, false);
        if (selected is not IAntiquityChoice choice) return;
        await PowerCmd.Apply<AntiquitiesTreasureChosenPower>(context, Owner.Creature, 1m,
            Owner.Creature, selected, silent: true);
        Flash();
        switch (choice.Choice)
        {
            case AntiquityChoice.Np:
                await NpCharge.Gain(context, Owner.Creature, 30, selected);
                break;
            case AntiquityChoice.Sake:
                await SakeBank.Gain(context, Owner.Creature, 30, selected);
                break;
            case AntiquityChoice.Stars:
                await CritStars.Gain(context, Owner.Creature, 30, selected);
                break;
        }
    }
}
