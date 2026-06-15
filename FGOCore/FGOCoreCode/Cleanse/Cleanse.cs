using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Forms;

namespace FGOCore.FGOCoreCode.Cleanse;

/// <summary>
/// Marcador: este power es un RECURSO del jugador (medidor/banco), NUNCA debe ser barrido por un
/// cleanse de buffs/debuffs. Lo implementan los recursos de FGOCore (NP, Estrellas de Crítico,
/// Bendición de Sobrecarga) y los recursos por-personaje que casualmente sean Debuff-type
/// (p.ej. la Deuda de Oberon, que es PowerType.Debuff pero es un activo, no un debuff a limpiar).
/// </summary>
public interface IResourcePower;

/// <summary>
/// Cleanse compartido. Barre Debuffs o Buffs del objetivo SIEMPRE preservando los recursos
/// (<see cref="IResourcePower"/>) y las formas (<see cref="FormPower"/>). Reemplaza los
/// RemoveAllDebuffs duplicados y DIVERGENTES de Oberon/Siegfried/Artoria — varios borraban por
/// error la Deuda (Oberon) o los recursos (NP/Estrellas/Sobrecarga del strip de El Fin de los Sueños).
/// </summary>
public static class Cleanse
{
    /// <summary>Quita los Debuff efectivos del objetivo (signado vía TypeForCurrentAmount), salvo
    /// recursos (<see cref="IResourcePower"/>) y lo que excluya <paramref name="keep"/>. Devuelve cuántos quitó.</summary>
    public static async Task<int> RemoveDebuffs(Creature target, int max = int.MaxValue, Func<PowerModel, bool>? keep = null)
    {
        var toRemove = target.Powers
            .Where(p => p.TypeForCurrentAmount == PowerType.Debuff && p is not IResourcePower && (keep == null || !keep(p)))
            .Take(max).ToList();
        foreach (var power in toRemove) await PowerCmd.Remove(power);
        return toRemove.Count;
    }

    /// <summary>Quita los Buff positivos del objetivo, salvo formas (<see cref="FormPower"/>),
    /// recursos (<see cref="IResourcePower"/>) y lo que excluya <paramref name="keep"/>. Devuelve cuántos quitó.</summary>
    public static async Task<int> RemoveBuffs(Creature target, int max = int.MaxValue, Func<PowerModel, bool>? keep = null)
    {
        var toRemove = target.Powers
            .Where(p => p.TypeForCurrentAmount == PowerType.Buff && p is not FormPower && p is not IResourcePower && (keep == null || !keep(p)))
            .Take(max).ToList();
        foreach (var power in toRemove) await PowerCmd.Remove(power);
        return toRemove.Count;
    }
}
