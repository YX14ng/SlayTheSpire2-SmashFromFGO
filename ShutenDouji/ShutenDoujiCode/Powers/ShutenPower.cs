using BaseLib.Extensions;
using Godot;
using ShutenDouji.ShutenDoujiCode.Extensions;

namespace ShutenDouji.ShutenDoujiCode.Powers;

public abstract class ShutenPower : FGOCorePower
{
    private string SmallPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    private string BigPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public override string CustomPackedIconPath => ResourceLoader.Exists(SmallPath)
        ? SmallPath
        : "res://FGOCore/images/powers/power.png";

    public override string CustomBigIconPath => ResourceLoader.Exists(BigPath)
        ? BigPath
        : "res://FGOCore/images/powers/big/power.png";
}
