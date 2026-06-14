using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Estandarte de Camlann (卡姆兰旗帜) — reliquia RARA (DESIGN-MORDRED §6): en combates vs Élite/Jefe,
/// empezás con +<see cref="StartingNp"/> Carga NP y +<see cref="StartingStars"/> Estrellas de Crítico.
/// Engorda el rider anti-autoridad EXACTAMENTE donde importa (las peleas-blanco del trait [Arthur]),
/// honesta en pasillos (duerme). Reúso del check <see cref="MordredExtensions.VersusAuthority"/>
/// (el tipo de encuentro, patrón AntiPurgeCrystal). Sin ×global ni motor nuevo.
/// </summary>
public sealed class BannerOfCamlann : MordredRelic
{
    private const int StartingNp = 30;
    private const int StartingStars = 10;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        if (!Owner.Creature.VersusAuthority()) return;
        Flash();
        await NpCharge.Gain(Owner.Creature, StartingNp, null);
        await CritStars.Gain(Owner.Creature, StartingStars, null);
    }
}
