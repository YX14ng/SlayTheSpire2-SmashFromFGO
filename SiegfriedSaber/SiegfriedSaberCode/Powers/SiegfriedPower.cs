using BaseLib.Extensions;
using SiegfriedSaber.SiegfriedSaberCode.Extensions;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>Base de los powers de Siegfried: los íconos viven en los recursos de este mod
/// (espejo de TiamatPower).</summary>
public abstract class SiegfriedPower : FGOCorePower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
