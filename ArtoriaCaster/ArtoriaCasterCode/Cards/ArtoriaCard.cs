using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using ArtoriaCaster.ArtoriaCasterCode.Character;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

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

    /// <summary>
    /// Co-op: ejecuta <paramref name="perAlly"/> sobre cada aliado VIVO (excluido el propio dueño),
    /// en el orden de <c>RunState.Players</c>. Factoriza el bucle idéntico que repetían las cartas de
    /// party (Around Caliburn, Carisma de la Esperanza). El gasto/regalo por aliado lo decide el caller.
    /// </summary>
    protected async Task ForEachAlly(Func<Creature, Task> perAlly)
    {
        foreach (var player in Owner.RunState.Players)
        {
            if (player == Owner || player.Creature.IsDead) continue;
            await perAlly(player.Creature);
        }
    }

    /// <summary>
    /// Resuelve el daño de las cartas-Ataque de UN golpe con crítico: si el dueño puede criticar
    /// (<see cref="Stars.CanCrit"/>) gasta las estrellas y devuelve el valor de crítico (Crit +
    /// <see cref="Stars.CritBonus"/>); si no, devuelve el daño base. Era un bloque copiado palabra
    /// por palabra en las cartas de un solo golpe (Arrebato/Tajo/Embate/Juicio/Estrella Fugaz/
    /// Corte de Selección/Anhelo Heredado). Lee Damage/Crit de los DynamicVars de la propia carta.
    /// </summary>
    protected async Task<decimal> ResolveCritDamage(int critCost)
    {
        var damage = DynamicVars.Damage.BaseValue;
        if (Stars.CanCrit(Owner.Creature, critCost))
        {
            await Stars.ConsumeForCrit(Owner.Creature, critCost, this);
            damage = DynamicVars["Crit"].BaseValue + Stars.CritBonus(Owner.Creature);
        }
        return damage;
    }
}
