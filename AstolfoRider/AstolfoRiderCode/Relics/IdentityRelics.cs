using AstolfoRider.AstolfoRiderCode.Caprice;
using AstolfoRider.AstolfoRiderCode.Extensions;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AstolfoRider.AstolfoRiderCode.Relics;

public sealed class ReasonEvaporatedRelic : AstolfoRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<CompletelyEvaporatedReason>();
    public override async Task BeforeCombatStartLate()
    {
        if (Owner.Relics.Any(relic => relic is CompletelyEvaporatedReason)) return;
        var context = new BlockingPlayerChoiceContext();
        await Caprices.EnsureInstalled(context, Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
        await NpCharge.Gain(context, Owner.Creature, 30, null);
        await Caprices.Draw(context, Owner.Creature, null);
        await MainFile.EnsureNpInCombat(context, Owner.Creature);
    }
}

public sealed class CompletelyEvaporatedReason : AstolfoRelic, ICapriceFulfilledListener
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    public override async Task BeforeCombatStartLate()
    {
        var context = new BlockingPlayerChoiceContext();
        await Caprices.EnsureInstalled(context, Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
        await NpCharge.Gain(context, Owner.Creature, 50, null);
        await Caprices.Draw(context, Owner.Creature, null);
        await MainFile.EnsureNpInCombat(context, Owner.Creature);
    }
    public async Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment fulfillment)
    {
        if (fulfillment.Owner != Owner.Creature || fulfillment.NumberThisTurn != 1) return;
        Flash();
        await NpCharge.Gain(context, Owner.Creature, 10, fulfillment.Card);
        await CritStars.Gain(context, Owner.Creature, 10, fulfillment.Card);
    }
}

[Pool(typeof(Character.AstolfoRelicPool))]
public sealed class OathOfTheJoyfulPaladin : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    private string Packed => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    private string Outline => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    private string Big => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
    public override string PackedIconPath => ResourceLoader.Exists(Packed)
        ? Packed : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/burning_blood.tres");
    protected override string PackedIconOutlinePath => ResourceLoader.Exists(Outline)
        ? Outline : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/burning_blood.tres");
    protected override string BigIconPath => ResourceLoader.Exists(Big)
        ? Big : ImageHelper.GetImagePath("relics/burning_blood.png");
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<EvasionPower>()];
    protected override int StartingNp(int level) => level >= 4 ? 20 : 0;
    protected override int StartingBlock(int level) => 0;

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        var context = new BlockingPlayerChoiceContext();
        await Caprices.EnsureInstalled(context, Owner.Creature);
        if (Level >= 7) await CritStars.Gain(context, Owner.Creature, 20, null);
        await MainFile.EnsureNpInCombat(context, Owner.Creature);
    }

    protected override async Task ApplyCapstone()
    {
        var context = new BlockingPlayerChoiceContext();
        var players = Owner.Creature.CombatState?.PlayerCreatures ?? [];
        foreach (var player in players.Where(player => !player.IsDead))
            await Evasion.Grant(context, player, 1, null);
        await NpCharge.Gain(context, Owner.Creature, 10, null);
    }
}

public sealed class BookOfTheForgottenName : AstolfoRelic, INpLevelStore
{
    public const string DupeOptionId = "ASTOLFO_DUPE";
    private int _npLevel = 1;
    private int _dupePity;
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => NpLevel;

    [SavedProperty]
    public int NpLevel
    {
        get => _npLevel;
        set { AssertMutable(); _npLevel = value; InvokeDisplayAmountChanged(); }
    }
    [SavedProperty]
    public int DupePity
    {
        get => _dupePity;
        set { AssertMutable(); _dupePity = value; }
    }
    public override bool TryModifyCardRewardAlternatives(
        Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)
    {
        if (Owner != player || alternatives.Count >= 3 || !NpLevels.CanLevelUp(Owner)) return false;
        alternatives.Add(new CardRewardAlternative(
            DupeOptionId, OnDupeRoll, PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }
    private async Task OnDupeRoll()
    {
        if (await NpLevels.TryRollDupeWithConsolation(Owner)) Flash();
    }
}

public sealed class ChaliceOfTheTwelvePaladins : AstolfoRelic, ILimitBreaker
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new MaxHpVar(15m)];
    public int ExtraBondLevels => 2;
    public int ExtraNpLevels => 1;
    public override async Task AfterObtained()
    {
        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }
}
