using AstolfoRider.AstolfoRiderCode.Character;
using AstolfoRider.AstolfoRiderCode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AstolfoRider.AstolfoRiderCode.Cards;

[Pool(typeof(AstolfoCardPool))]
public abstract class AstolfoCard(
    int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    private string PortraitAsset => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    private string BigPortraitAsset => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string CustomPortraitPath => ResourceLoader.Exists(BigPortraitAsset)
        ? BigPortraitAsset : "res://FGOCore/images/card_portraits/big/card.png";
    public override string PortraitPath => ResourceLoader.Exists(PortraitAsset)
        ? PortraitAsset : "res://FGOCore/images/card_portraits/card.png";
}

public abstract class AstolfoCommandCard(
    int cost, CardType type, CardRarity rarity, TargetType target, CommandType commandType) :
    AstolfoCard(cost, type, rarity, target), ICommandTyped
{
    [SavedProperty]
    public CommandType CommandType { get; protected set; } = commandType;
    public virtual bool IsNoblePhantasm => false;
    public virtual int DamagePortions => 1;
    internal decimal CapriceDamageBonusTotal { get; set; }
    internal decimal ExternalDamageBonusTotal { get; set; }
    protected decimal WithCapriceDamage(decimal amount, int portions = 1) =>
        amount + (CapriceDamageBonusTotal + ExternalDamageBonusTotal) / Math.Max(1, portions);
    protected bool MatchesCaprice =>
        Owner?.Creature != null && Caprice.Caprices.Matches(Owner.Creature, CommandType);
    protected override bool ShouldGlowGoldInternal => !IsNoblePhantasm && MatchesCaprice;

    public void OverrideCommandType(CommandType type) => CommandType = type;
}
