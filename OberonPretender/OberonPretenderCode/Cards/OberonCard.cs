using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using OberonPretender.OberonPretenderCode.Character;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Cards;

[Pool(typeof(OberonCardPool))]
public abstract class OberonCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
