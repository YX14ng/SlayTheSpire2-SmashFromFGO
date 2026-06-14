using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Santo Grial de la Capital Imperial (帝都圣杯) — reliquia RARA/EVENTO (DESIGN-OKITA §6.2): Koha-Ace,
/// la Extraña Historia del Grial de la Capital Imperial — SU grial. Al obtenerla, +15 HP máx. permanente;
/// mientras la tengas, rompe los topes del sistema FGO (ILimitBreaker): el Vínculo llega a Nv12 (+5 HP
/// máx. por nivel extra, vía BondRelic) y el nivel de NP llega a 6. Patrón HolyGrail de Mash.
/// </summary>
public sealed class HolyGrailImperialCapital : OkitaRelic, ILimitBreaker
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
