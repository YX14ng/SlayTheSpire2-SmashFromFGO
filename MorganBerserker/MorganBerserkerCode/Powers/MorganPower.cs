using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>Base for Morgan's powers; icons load from this mod's resources.</summary>
public abstract class MorganPower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public abstract override PowerType Type { get; }

    public abstract override PowerStackType StackType { get; }
}
