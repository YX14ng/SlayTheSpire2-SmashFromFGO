using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using GilgameshArcher.GilgameshArcherCode.Character;
using GilgameshArcher.GilgameshArcherCode.Extensions;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

[Pool(typeof(GilgameshRelicPool))]
public abstract class GilgameshRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
