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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using ShutenDouji.ShutenDoujiCode.Character;
using ShutenDouji.ShutenDoujiCode.Extensions;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Relics;

public sealed class ScarletGourd : ShutenRelic, IStylePlayedListener
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStartLate()
    {
        if (Owner.Relics.Any(relic => relic is InexhaustibleGourd)) return;
        var context = new BlockingPlayerChoiceContext();
        await StyleHistoryPower.EnsureInstalled(context, Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
        await SakeBank.Gain(context, Owner.Creature, 20, null);
    }

    public async Task AfterStylePlayed(PlayerChoiceContext context, StylePlay play)
    {
        if (!play.FirstOfStyle || play.Owner != Owner.Creature ||
            Owner.Relics.Any(relic => relic is InexhaustibleGourd)) return;
        Flash();
        await SakeBank.Gain(context, Owner.Creature, 10, play.Card);
    }
}

public sealed class InexhaustibleGourd : ShutenRelic, IStylePlayedListener
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeCombatStartLate()
    {
        var context = new BlockingPlayerChoiceContext();
        await StyleHistoryPower.EnsureInstalled(context, Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
        await SakeBank.Gain(context, Owner.Creature, 30, null);
    }

    public async Task AfterStylePlayed(PlayerChoiceContext context, StylePlay play)
    {
        if (!play.FirstOfStyle || play.Owner != Owner.Creature) return;
        Flash();
        await SakeBank.Gain(context, Owner.Creature, 20, play.Card);
    }
}

[Pool(typeof(ShutenRelicPool))]
public sealed class BanquetOath : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    private string SmallPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    private string OutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    private string BigPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    public override string PackedIconPath => ResourceLoader.Exists(SmallPath)
        ? SmallPath : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/burning_blood.tres");
    protected override string PackedIconOutlinePath => ResourceLoader.Exists(OutlinePath)
        ? OutlinePath : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/burning_blood.tres");
    protected override string BigIconPath => ResourceLoader.Exists(BigPath)
        ? BigPath : ImageHelper.GetImagePath("relics/burning_blood.png");

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SakePower>(),
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CritStarsPower>()
    ];

    protected override int StartingNp(int level) => level >= 7 ? 20 : 0;
    protected override int StartingBlock(int level) => 0;

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        var context = new BlockingPlayerChoiceContext();
        await StyleHistoryPower.EnsureInstalled(context, Owner.Creature);
        if (Level >= 4) await SakeBank.Gain(context, Owner.Creature, 20, null);
        if (Level >= 7) await CritStars.Gain(context, Owner.Creature, 20, null);
        await MainFile.EnsureNpsInCombat(context, Owner.Creature);
    }

    protected override async Task ApplyCapstone()
    {
        var context = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<ArtifactPower>(context, Owner.Creature, 1m, Owner.Creature, null);
        await SakeBank.Gain(context, Owner.Creature, 10, null);
    }
}

public sealed class SeveredHeadMemory : ShutenRelic, INpLevelStore
{
    public const string DupeOptionId = "SHUTEN_DUPE";
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

public sealed class KuzuryuChalice : ShutenRelic, ILimitBreaker
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
