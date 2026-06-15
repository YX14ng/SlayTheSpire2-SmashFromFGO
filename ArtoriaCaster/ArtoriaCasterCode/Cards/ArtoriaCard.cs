using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using ArtoriaCaster.ArtoriaCasterCode.Character;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards;

[Pool(typeof(ArtoriaCardPool))]
public abstract class ArtoriaCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>
    /// Removes every debuff on the owner (Around Caliburn's cleanse). Delegates to the
    /// shared <see cref="Cleanse.RemoveDebuffs"/> so player resources (IResourcePower:
    /// NP/Stars/Overcharge) are never swept — single source of truth across FGO mods.
    /// </summary>
    protected Task RemoveOwnDebuffs() => Cleanse.RemoveDebuffs(Owner.Creature);
}
