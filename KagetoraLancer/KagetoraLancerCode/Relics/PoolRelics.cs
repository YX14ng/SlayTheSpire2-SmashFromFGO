using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Relics;

public sealed class SakeCup : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.SakeCup) || player != Owner) return;
        await KagetoraUsages.Mark(context, Owner.Creature, KagetoraUsage.SakeCup, null);
        await CardPileCmd.Draw(context, 1, Owner);
        var selected = await CardSelectCmd.FromHandForDiscard(context, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this);
        await CardCmd.Discard(context, selected);
    }
}

public sealed class EightPetalBanner : KagetoraRelic, IDoctrineAdvanceListener
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.EightPetalBanner) ||
            !result.Advanced) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.EightPetalBanner, result.CardPlay.Card);
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, 2m, ValueProp.Unpowered, null);
    }
}

public sealed class HoushoutsukigeReins : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.HoushoutsukigeReins) ||
            cardPlay.Card.Owner != Owner || cardPlay.Card is not IPreceptCard { Precept: Precept.Feet }) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.HoushoutsukigeReins, cardPlay.Card);
        Flash();
        await CritStars.Gain(context, Owner.Creature, 10, cardPlay.Card);
    }
}

public sealed class SixPlateArmour : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.SixPlateArmour) ||
            cardPlay.Card.Owner != Owner || cardPlay.Card is not IPreceptCard { Precept: Precept.Chest }) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.SixPlateArmour, cardPlay.Card);
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, 4m, ValueProp.Unpowered, null);
    }
}

public sealed class ShiranuiTachi : KagetoraRelic, ICriticalConsumedListener
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.ShiranuiTachi) ||
            critical.Owner != Owner.Creature) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.ShiranuiTachi, critical.Card);
        Flash();
        await NpCharge.Gain(context, Owner.Creature, 10, critical.Card);
    }
}

public sealed class WhiteFlameBrazier : KagetoraRelic, IAscensionListener
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public async Task AfterAscendingToKenshin(PlayerChoiceContext context, CardModel source)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.WhiteFlameBrazier)) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.WhiteFlameBrazier, source);
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
        await CritStars.Gain(context, Owner.Creature, 30, source);
    }
}

public sealed class EchigoSaltBag : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override async Task BeforeCombatStartLate()
    {
        var players = Owner.Creature.CombatState?.PlayerCreatures;
        if (players == null) return;
        foreach (var creature in players)
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), creature, 1m, Owner.Creature, null);
        var allies = Math.Max(0, players.Count - 1);
        if (allies > 0) await NpCharge.Gain(Owner.Creature, allies * 10, null);
    }
}
