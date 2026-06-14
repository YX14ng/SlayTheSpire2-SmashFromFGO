using FGOCore.FGOCoreCode.Np;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// El Cáliz Original (原典圣杯) — reliquia RARA/EVENTO (DESIGN-GILGAMESH §6), el limit-breaker estándar del
/// roster con flavor de Gil («¿Un Santo Grial? Mestizo, tengo MILES»). Al obtenerla: +15 de HP máx
/// permanente; mientras la tengas rompe los topes del sistema FGO (<see cref="ILimitBreaker"/>): el Vínculo
/// puede llegar a nivel 12 (ExtraBondLevels = 2) y el nivel de NP a 6 (ExtraNpLevels = 1). Patrón
/// HolyGrailOfSelection (Mordred) / HolyGrail (Mash).
/// </summary>
public sealed class TheOriginalChalice : GilgameshRelic, ILimitBreaker
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
