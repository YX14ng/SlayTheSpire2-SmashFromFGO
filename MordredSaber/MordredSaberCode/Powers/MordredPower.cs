using BaseLib.Extensions;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>Base de los powers de Mordred: los íconos viven en los recursos de este mod
/// (espejo de TiamatPower).</summary>
public abstract class MordredPower : FGOCorePower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
