using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using KagetoraLancer.KagetoraLancerCode.Character;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Extensions;

namespace KagetoraLancer.KagetoraLancerCode.Cards;

[Pool(typeof(KagetoraCardPool))]
public abstract class KagetoraCard(
    int cost, CardType type, CardRarity rarity, TargetType target, Precept precept = Precept.None) :
    CustomCardModel(cost, type, rarity, target), IPreceptCard
{
    public Precept Precept { get; protected set; } = precept;
    private string CardPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    private string BigCardPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string CustomPortraitPath => ResourceLoader.Exists(BigCardPortraitPath)
        ? BigCardPortraitPath
        : "res://FGOCore/images/card_portraits/big/card.png";

    public override string PortraitPath => ResourceLoader.Exists(CardPortraitPath)
        ? CardPortraitPath
        : "res://FGOCore/images/card_portraits/card.png";
}
