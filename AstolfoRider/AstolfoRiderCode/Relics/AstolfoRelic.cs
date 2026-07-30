using AstolfoRider.AstolfoRiderCode.Character;
using AstolfoRider.AstolfoRiderCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace AstolfoRider.AstolfoRiderCode.Relics;

[Pool(typeof(AstolfoRelicPool))]
public abstract class AstolfoRelic : CustomRelicModel
{
    private string Packed => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    private string Outline => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    private string Big => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
    public override string PackedIconPath => ResourceLoader.Exists(Packed)
        ? Packed : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/burning_blood.tres");
    protected override string PackedIconOutlinePath => ResourceLoader.Exists(Outline)
        ? Outline : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/burning_blood.tres");
    protected override string BigIconPath => ResourceLoader.Exists(Big)
        ? Big : ImageHelper.GetImagePath("relics/burning_blood.png");
}
