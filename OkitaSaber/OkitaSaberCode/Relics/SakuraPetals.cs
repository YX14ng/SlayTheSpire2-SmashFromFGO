using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Pétalos de Sakura (樱花花瓣) — reliquia POCO COMÚN (DESIGN-OKITA §6.2): al inicio de cada combate,
/// +30 *Estrellas de Crítico. Adelanta el primer umbral de 100★ (~⅓ del camino regalado). Reúso puro
/// de la economía de estrellas de FGOCore (CritStars.Gain).
/// </summary>
public sealed class SakuraPetals : OkitaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int Stars = 30;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Flash();
        await CritStars.Gain(Owner.Creature, Stars, null);
    }
}
