using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Santo Grial de la Selección (选定的圣杯) — reliquia RARA/EVENTO (DESIGN-MORDRED §6), el Grial que
/// Mordred reclama en la Gran Guerra del Grial. Al obtenerla: +<see cref="DynamicVars"/> 15 de HP máx
/// permanente; mientras la tengas rompe los topes del sistema FGO (FGOCore <see cref="ILimitBreaker"/>):
/// el Vínculo puede llegar a nivel 12 y el nivel de NP a 6. Patrón HolyGrail de Mash /
/// InnerSeaChalice de Artoria / LadyOfTheLakeChalice de Morgan.
/// </summary>
public sealed class HolyGrailOfSelection : MordredRelic, ILimitBreaker
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
