using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using FGOCore.FGOCoreCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// FGO community meme cards (mainly from the Chinese community: Mooncell's 黑话·梗 canon).
/// Colorless pool: they can show up in anyone's run, not just FGO characters'.
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public abstract class MemeCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
