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

/// <summary>
/// Reliquia inicial (E3, §8) — <b>+10★ por cada AVANCE de la Doctrina</b>, exactamente 3 procs por
/// turno, <b>sin robo</b>.
///
/// <para><b>El cap de 3 procs/turno NO está implementado y no hay que implementarlo</b> (§16.5): es
/// una CONSECUENCIA de <c>DoctrinePower.MaxAdvancesPerTurn = 3</c>, evaluado en la primera línea de
/// <c>WouldAdvance</c> y otra vez en el guard de <c>AfterCardPlayed</c>, las dos veces ANTES de
/// cualquier <c>IDoctrineFailureOverride</c>. Agregar acá un contador redundante sería superficie de
/// bug y de desincronización con el motor. Que nadie lo «arregle».</para>
///
/// <para>Antes proceaba una sola vez por CICLO: un tercio del caudal contra el que se calibró el
/// pool (E4: 10+10+10 de la Pagoda + 20 del innato de Pies = 50★ = <c>CritStarsPower.CritCost</c>,
/// el crítico del turno).</para>
///
/// <para><b>El robo se fue</b> (parche J1 P-2, §12.3-1): con E1 el cierre ya devuelve 1⚡, y un ciclo
/// repetible que paga ⚡ <i>y</i> carta es la bandera roja n.º 1 del rúbrico. El robo sobrevive en la
/// Gran Pagoda, que es reliquia de jefe y está pagada.</para>
/// </summary>
public sealed class JeweledPagodaOfBishamonten : KagetoraRelic, IDoctrineAdvanceListener
{
    public const int StarsPerAdvance = 10;

    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DoctrinePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<GreatPagodaOfBishamonten>();

    public override async Task BeforeCombatStartLate()
    {
        // Forma inicial: source == null no cuenta como una transformacion.
        await FormSwitch.Enter<NagaoKagetoraFormPower>(null, Owner.Creature, null);
        await DoctrinePower.EnsureInstalled(Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
    }

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        // La Gran Pagoda hereda el mismo evento; el guard evita el doble proceo si por cualquier
        // camino las dos conviven en la bolsa.
        if (!result.Advanced || Owner.Relics.Any(relic => relic is GreatPagodaOfBishamonten)) return;
        Flash();
        await CritStars.Gain(context, Owner.Creature, StarsPerAdvance, result.CardPlay.Card);
    }
}

/// <summary>
/// Intercambio Ancient (§8): las mismas <see cref="JeweledPagodaOfBishamonten.StarsPerAdvance"/> por
/// avance, <b>y al completar un ciclo: robá 1 y +10 de Carga NP</b>. Acá sí vive el robo — es
/// reliquia de jefe (§12.3-1). Reinstala forma / Doctrina / CommandBonus como la base, que es el
/// contrato de DECISIONS para un reemplazo Ancient de una starter.
/// </summary>
public sealed class GreatPagodaOfBishamonten : KagetoraRelic, IDoctrineAdvanceListener, IDoctrineCycleListener
{
    public const int CycleNpCharge = 10;

    public override RelicRarity Rarity => RelicRarity.Ancient;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoctrinePower>(), HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    public override async Task BeforeCombatStartLate()
    {
        await FormSwitch.Enter<NagaoKagetoraFormPower>(null, Owner.Creature, null);
        await DoctrinePower.EnsureInstalled(Owner.Creature);
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
    }

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (!result.Advanced) return;
        Flash();
        await CritStars.Gain(
            context, Owner.Creature, JeweledPagodaOfBishamonten.StarsPerAdvance, result.CardPlay.Card);
    }

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        // El Flash va ANTES del chequeo del mazo: completar un ciclo es el evento central del
        // personaje y con el mazo de robo vacío quedaba COMPLETAMENTE silencioso (bugfix
        // 2026-08-16). El guard del mazo se conserva: robar con el mazo vacío gatilla el reshuffle
        // que puede corromper la carta en curso (patrón anti-soft-lock del repo).
        Flash();
        await NpCharge.Gain(context, Owner.Creature, CycleNpCharge, result.CardPlay.Card);
        if (PileType.Draw.GetPile(Owner).Cards.Count <= 0) return;
        await CardPileCmd.Draw(context, 1, Owner);
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
