using AstolfoRider.AstolfoRiderCode.Extensions;
using BaseLib.Extensions;
using Godot;

namespace AstolfoRider.AstolfoRiderCode.Powers;

public abstract class AstolfoPower : FGOCorePower
{
    private string PackedPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    private string BigPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    public override string CustomPackedIconPath => ResourceLoader.Exists(PackedPath)
        ? PackedPath : "res://FGOCore/images/powers/power.png";
    public override string CustomBigIconPath => ResourceLoader.Exists(BigPath)
        ? BigPath : "res://FGOCore/images/powers/big/power.png";
}
