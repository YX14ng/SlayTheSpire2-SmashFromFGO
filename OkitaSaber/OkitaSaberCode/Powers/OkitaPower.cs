using BaseLib.Extensions;
using OkitaSaber.OkitaSaberCode.Extensions;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>Base de los powers de Okita: los íconos viven en los recursos de este mod
/// (espejo de TiamatPower).</summary>
public abstract class OkitaPower : FGOCorePower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
