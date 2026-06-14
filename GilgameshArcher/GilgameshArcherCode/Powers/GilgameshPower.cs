using BaseLib.Extensions;
using GilgameshArcher.GilgameshArcherCode.Extensions;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>Base de los powers de Gilgamesh: los íconos viven en los recursos de este mod
/// (espejo de TiamatPower).</summary>
public abstract class GilgameshPower : FGOCorePower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
