using FGOCore.FGOCoreCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Santo Grial de las Hadas (妖精的圣杯 / Holy Grail of the Fae) — reliquia RARA/EVENTO de
/// Palingenesis (DESIGN-OBERON §7 #11): al obtenerla, +15 de Vida maxima permanente; mientras la tengas,
/// rompe los topes del sistema FGO — el Vinculo (Cronica de Avalon) puede llegar a Nv12
/// (+<see cref="ExtraBondLevels"/>) y el nivel de NP (Nomeolvides) puede llegar a 6
/// (+<see cref="ExtraNpLevels"/>), via <see cref="ILimitBreaker"/> de FGOCore. Patron HolyGrail de Mash.
/// </summary>
public sealed class HolyGrailOfTheFae : OberonRelic, ILimitBreaker
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new MaxHpVar(15m)];

    public int ExtraBondLevels => 2;

    public int ExtraNpLevels => 1;

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }
}
