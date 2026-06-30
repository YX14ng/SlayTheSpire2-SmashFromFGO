using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Black Barrel — the Atlas Institute's conceptual cannon. Black Barrel damage
/// ignores enemy Block and strips one buff from the target ("kills the immortal").
/// </summary>
public static class BlackBarrel
{
    public static async Task Hit(PlayerChoiceContext choiceContext, Creature target, decimal amount, Creature dealer, CardModel source)
    {
        VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_dramatic_stab");
        await CreatureCmd.Damage(choiceContext, target, amount, ValueProp.Move | ValueProp.Unblockable, dealer, source);
        await RemoveBuffs(target, 1, dealer, source);
    }

    /// <summary>
    /// Quita hasta <paramref name="count"/> buffs del objetivo y devuelve cuántos quitó. Si pasás
    /// el <paramref name="stripper"/> (el jugador atacante) y ése tiene <see cref="ConceptualAmmoPower"/>,
    /// otorga sus Estrellas de Crítico por cada buff efectivamente removido (payoff del eje de strip).
    /// </summary>
    public static async Task<int> RemoveBuffs(Creature target, int count, Creature? stripper = null, CardModel? source = null)
    {
        var removed = 0;
        for (var i = 0; i < count; i++)
        {
            var buff = target.GetPowerInstances<PowerModel>().FirstOrDefault(IsStrippableBuff);
            if (buff == null) break;
            await PowerCmd.Remove(buff);
            removed++;
        }

        if (removed > 0 && stripper != null)
        {
            var ammo = stripper.GetPowerInstances<ConceptualAmmoPower>().FirstOrDefault();
            if (ammo != null) await ammo.OnBuffsStripped(removed, source);
        }

        return removed;
    }

    public static async Task<int> RemoveAllBuffs(Creature target, Creature? stripper = null, CardModel? source = null)
    {
        return await RemoveBuffs(target, int.MaxValue, stripper, source);
    }

    /// <summary>
    /// Black Barrel strips a real COMBAT buff (Fuerza, Plating, Buffer, regen, etc.), NOT the
    /// structural/identity powers that drive enemy behaviour. Removing those mid-combat breaks the
    /// game — reportado: quitar el *Personal Hive* del Entomancer cuelga su Pheromone Spit; quitar
    /// *Illusion* de un invocado lo vuelve inmortal y cuelga; *Minion* (invocados). Por eso los
    /// excluimos, además de cualquier buff sobre un enemigo secundario (invocado). Fail-safe: ante
    /// la duda, no se quita. Si aparecen otros powers estructurales que rompan, se suman acá.
    /// </summary>
    private static bool IsStrippableBuff(PowerModel p)
        => p.Type == PowerType.Buff
           && p is not MinionPower
           && p is not IllusionPower
           && p is not PersonalHivePower
           && !p.OwnerIsSecondaryEnemy;
}
