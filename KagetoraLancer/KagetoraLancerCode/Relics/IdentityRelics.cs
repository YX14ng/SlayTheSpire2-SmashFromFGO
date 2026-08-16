using BaseLib.Extensions;
using Godot;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Extensions;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace KagetoraLancer.KagetoraLancerCode.Relics;

/// <summary>Reliquia inicial: la recompensa de ciclo roba una carta.</summary>
public sealed class JeweledPagodaOfBishamonten : KagetoraRelic, IDoctrineCycleListener
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<GreatPagodaOfBishamonten>();

    public override async Task BeforeCombatStartLate()
    {
        // Forma inicial: source == null no cuenta como una transformacion.
        await FormSwitch.Enter<NagaoKagetoraFormPower>(null, Owner.Creature, null);
        await DoctrinePower.EnsureInstalled(Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
    }

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (Owner.Relics.Any(relic => relic is GreatPagodaOfBishamonten)) return;
        // El Flash va ANTES del chequeo del mazo: completar un ciclo es el evento central del
        // personaje y con el mazo de robo vacío quedaba COMPLETAMENTE silencioso (bugfix
        // 2026-08-16). El guard del mazo se conserva: robar con el mazo vacío gatilla el reshuffle
        // que puede corromper la carta en curso (patrón anti-soft-lock del repo).
        Flash();
        var drawPile = PileType.Draw.GetPile(Owner);
        if (drawPile.Cards.Count <= 0) return;
        await CardPileCmd.Draw(context, 1, Owner);
    }
}

/// <summary>Intercambio Ancient: conserva el mismo evento y roba dos.</summary>
public sealed class GreatPagodaOfBishamonten : KagetoraRelic, IDoctrineCycleListener
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeCombatStartLate()
    {
        await FormSwitch.Enter<NagaoKagetoraFormPower>(null, Owner.Creature, null);
        await DoctrinePower.EnsureInstalled(Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
    }

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        var draw = Math.Min(2, PileType.Draw.GetPile(Owner).Cards.Count);
        if (draw <= 0) return;
        Flash();
        await CardPileCmd.Draw(context, draw, Owner);
    }
}

/// <summary>Vínculo propio: estrellas en 4, NP en 7 y Bendición en 10.</summary>
[Pool(typeof(Character.KagetoraRelicPool))]
public sealed class OathOfEchigo : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    private string CustomPackedPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    private string CustomOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    private string CustomBigPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    public override string PackedIconPath => ResourceLoader.Exists(CustomPackedPath)
        ? CustomPackedPath
        : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/burning_blood.tres");
    protected override string PackedIconOutlinePath => ResourceLoader.Exists(CustomOutlinePath)
        ? CustomOutlinePath
        : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/burning_blood.tres");
    protected override string BigIconPath => ResourceLoader.Exists(CustomBigPath)
        ? CustomBigPath
        : ImageHelper.GetImagePath("relics/burning_blood.png");
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<BishamontenBlessingPower>()];

    protected override int StartingNp(int level) => level >= 7 ? 20 : 0;
    protected override int StartingBlock(int level) => 0;

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await DoctrinePower.EnsureInstalled(Owner.Creature);
        if (Level >= 4) await CritStars.Gain(Owner.Creature, 10, null);
        await MainFile.EnsureNpInCombat(Owner.Creature);
    }

    protected override Task ApplyCapstone() =>
        BishamontenBlessingPower.Grant(
            new BlockingPlayerChoiceContext(), Owner.Creature, null);
}

/// <summary>Almacén oculto del nivel de NP y su piedad.</summary>
public sealed class RecordOfEightFormations : KagetoraRelic, INpLevelStore
{
    public const string DupeOptionId = "KAGETORA_DUPE";
    private int _npLevel = 1;
    private int _dupePity;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => NpLevel;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>()];

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
        return NpDupeAlternative.TryAdd(
            Owner, player, alternatives, DupeOptionId, OnDupeRoll);
    }

    private async Task OnDupeRoll()
    {
        if (await NpLevels.TryRollDupeWithConsolation(Owner)) Flash();
    }
}

/// <summary>Grial estándar del roster.</summary>
public sealed class CommandersHolyGrail : KagetoraRelic, ILimitBreaker
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
