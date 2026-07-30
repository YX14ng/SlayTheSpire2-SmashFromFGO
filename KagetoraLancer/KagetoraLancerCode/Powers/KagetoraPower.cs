using BaseLib.Extensions;
using Godot;
using KagetoraLancer.KagetoraLancerCode.Extensions;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

public abstract class KagetoraPower : FGOCorePower
{
    private string PackedPowerPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    private string BigPowerPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public override string CustomPackedIconPath => ResourceLoader.Exists(PackedPowerPath)
        ? PackedPowerPath
        : "res://FGOCore/images/powers/power.png";

    public override string CustomBigIconPath => ResourceLoader.Exists(BigPowerPath)
        ? BigPowerPath
        : "res://FGOCore/images/powers/big/power.png";
}
