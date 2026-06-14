using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using OkitaSaber.OkitaSaberCode.Character;
using OkitaSaber.OkitaSaberCode.Extensions;

namespace OkitaSaber.OkitaSaberCode.Cards;

[Pool(typeof(OkitaCardPool))]
public abstract class OkitaCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
