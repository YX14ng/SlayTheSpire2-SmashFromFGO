using BaseLib.Extensions;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>Base de los powers de Oberon: los íconos viven en los recursos de este mod
/// (espejo de TiamatPower).</summary>
public abstract class OberonPower : FGOCorePower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
