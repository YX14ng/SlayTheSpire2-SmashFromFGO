using FGOCore.FGOCoreCode.Cleanse;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ShutenDouji.ShutenDoujiCode.Powers;

public sealed class DualNpManifestedPower : ShutenPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}
