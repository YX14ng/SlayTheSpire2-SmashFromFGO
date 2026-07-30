using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using ShutenDouji.ShutenDoujiCode.Character;
using ShutenDouji.ShutenDoujiCode.Extensions;

namespace ShutenDouji.ShutenDoujiCode.Relics;

[Pool(typeof(ShutenRelicPool))]
public abstract class ShutenRelic : CustomRelicModel
{
    private string SmallPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    private string OutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    private string BigPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    public override string PackedIconPath => ResourceLoader.Exists(SmallPath)
        ? SmallPath
        : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/burning_blood.tres");

    protected override string PackedIconOutlinePath => ResourceLoader.Exists(OutlinePath)
        ? OutlinePath
        : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/burning_blood.tres");

    protected override string BigIconPath => ResourceLoader.Exists(BigPath)
        ? BigPath
        : ImageHelper.GetImagePath("relics/burning_blood.png");
}
