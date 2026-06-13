using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using SiegfriedSaber.SiegfriedSaberCode.Character;
using SiegfriedSaber.SiegfriedSaberCode.Extensions;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

[Pool(typeof(SiegfriedRelicPool))]
public abstract class SiegfriedRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
