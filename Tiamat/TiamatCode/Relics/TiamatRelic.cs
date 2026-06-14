using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using TiamatBeast.TiamatCode.Character;
using TiamatBeast.TiamatCode.Extensions;

namespace TiamatBeast.TiamatCode.Relics;

[Pool(typeof(TiamatRelicPool))]
public abstract class TiamatRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
