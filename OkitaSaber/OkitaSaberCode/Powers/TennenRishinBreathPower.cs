using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Respiración del Tennen Rishin-ryū (天然理心流之息) — +<see cref="ExtraBreathCap"/> al tope de
/// *Aliento y recuperás <see cref="AlientoPower.RegenPerTurn"/>+<see cref="ExtraBreathRegen"/> de
/// Aliento por turno (3 en vez de 2) (DESIGN-OKITA §5.4). El motor del arquetipo Ráfaga.
/// AlientoPower lee estos boosters dinámicamente cada turno (no muta sus campos). Single.
/// </summary>
public sealed class TennenRishinBreathPower : OkitaPower, IBreathCapBooster, IBreathRegenBooster
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public int ExtraBreathCap => 2;

    public int ExtraBreathRegen => 1; // 2 base + 1 = 3/turno

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];
}
