using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using ArtoriaCaster.ArtoriaCasterCode.Character;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

[Pool(typeof(ArtoriaRelicPool))]
public abstract class ArtoriaRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
