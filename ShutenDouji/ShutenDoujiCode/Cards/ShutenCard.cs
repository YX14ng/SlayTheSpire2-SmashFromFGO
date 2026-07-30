using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using ShutenDouji.ShutenDoujiCode.Character;
using ShutenDouji.ShutenDoujiCode.Extensions;
using ShutenDouji.ShutenDoujiCode.Styles;

namespace ShutenDouji.ShutenDoujiCode.Cards;

[Pool(typeof(ShutenCardPool))]
public abstract class ShutenCard(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    ShutenStyle style) : CustomCardModel(cost, type, rarity, target), IShutenStyleCard
{
    public ShutenStyle Style { get; } = style;
    public virtual bool IsShutenNp => false;

    private string SmallPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    private string BigPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string CustomPortraitPath => ResourceLoader.Exists(BigPath)
        ? BigPath
        : "res://FGOCore/images/card_portraits/big/card.png";

    public override string PortraitPath => ResourceLoader.Exists(SmallPath)
        ? SmallPath
        : "res://FGOCore/images/card_portraits/card.png";

    protected bool HasCross => StyleState.HasCross(Owner.Creature, Style);
}
