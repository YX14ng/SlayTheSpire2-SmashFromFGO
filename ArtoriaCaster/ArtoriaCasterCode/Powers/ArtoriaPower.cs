using BaseLib.Extensions;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>Base for Artoria's powers: icons live in this mod's resources.</summary>
public abstract class ArtoriaPower : FGOCorePower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
