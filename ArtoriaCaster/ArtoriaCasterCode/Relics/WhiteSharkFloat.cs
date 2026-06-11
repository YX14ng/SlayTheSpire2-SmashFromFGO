using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Flotador de Tiburón Blanco — shop relic: start each combat with 3 Critical Stars.
/// </summary>
public sealed class WhiteSharkFloat : ArtoriaRelic
{
    public const int StartingStars = 3;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await Stars.Gain(Owner.Creature, StartingStars, null);
    }
}
