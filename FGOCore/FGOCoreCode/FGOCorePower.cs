using BaseLib.Abstracts;
using BaseLib.Extensions;
using FGOCore.FGOCoreCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace FGOCore.FGOCoreCode;

/// <summary>Base for FGOCore's shared powers; icons load from FGOCore's resources.</summary>
public abstract class FGOCorePower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public abstract override PowerType Type { get; }

    public abstract override PowerStackType StackType { get; }
}
