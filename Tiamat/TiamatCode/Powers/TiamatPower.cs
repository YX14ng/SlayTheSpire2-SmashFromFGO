using BaseLib.Extensions;
using TiamatBeast.TiamatCode.Extensions;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>Base de los powers de Tiamat: los íconos viven en los recursos de este mod.</summary>
public abstract class TiamatPower : FGOCorePower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
