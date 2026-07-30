using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using KagetoraLancer.KagetoraLancerCode.Character;
using KagetoraLancer.KagetoraLancerCode.Extensions;
using MegaCrit.Sts2.Core.Helpers;

namespace KagetoraLancer.KagetoraLancerCode.Relics;

[Pool(typeof(KagetoraRelicPool))]
public abstract class KagetoraRelic : CustomRelicModel
{
    private string CustomPackedPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    private string CustomOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    private string CustomBigPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    public override string PackedIconPath => ResourceLoader.Exists(CustomPackedPath)
        ? CustomPackedPath
        : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/burning_blood.tres");

    protected override string PackedIconOutlinePath => ResourceLoader.Exists(CustomOutlinePath)
        ? CustomOutlinePath
        : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/burning_blood.tres");

    protected override string BigIconPath => ResourceLoader.Exists(CustomBigPath)
        ? CustomBigPath
        : ImageHelper.GetImagePath("relics/burning_blood.png");
}
