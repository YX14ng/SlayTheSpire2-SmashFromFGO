using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using GilgameshArcher.GilgameshArcherCode.Character;
using GilgameshArcher.GilgameshArcherCode.Extensions;

namespace GilgameshArcher.GilgameshArcherCode.Cards;

[Pool(typeof(GilgameshCardPool))]
public abstract class GilgameshCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
