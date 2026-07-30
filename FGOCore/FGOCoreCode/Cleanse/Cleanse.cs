using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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
/// Marcador semántico para mejoras cuyo propósito principal es aumentar la ofensiva. Permite que
/// efectos selectivos (por ejemplo, los NP de Kagetora/Kenshin) las retiren sin borrar defensas,
/// recursos ni formas. Para powers sellados del juego base se puede usar
/// <see cref="Cleanse.RegisterOffensiveBuff{T}"/>.
/// </summary>
public interface IOffensiveBuffPower;

/// <summary>
/// Cleanse compartido. Barre Debuffs o Buffs del objetivo SIEMPRE preservando los recursos
/// (<see cref="IResourcePower"/>) y las formas (<see cref="FormPower"/>). Reemplaza los
/// RemoveAllDebuffs duplicados y DIVERGENTES de Oberon/Siegfried/Artoria — varios borraban por
/// error la Deuda (Oberon) o los recursos (NP/Estrellas/Sobrecarga del strip de El Fin de los Sueños).
/// </summary>
public static class Cleanse
{
    private static readonly HashSet<Type> OffensiveBuffTypes = [typeof(StrengthPower)];

    /// <summary>Registra un power sellado o externo como mejora ofensiva.</summary>
    public static void RegisterOffensiveBuff<T>() where T : PowerModel =>
        OffensiveBuffTypes.Add(typeof(T));

    /// <summary>Indica si el power es actualmente una mejora positiva marcada como ofensiva.</summary>
    public static bool IsOffensiveBuff(PowerModel power) =>
        power.TypeForCurrentAmount == PowerType.Buff &&
        power is not FormPower &&
        power is not IResourcePower &&
        (power is IOffensiveBuffPower || OffensiveBuffTypes.Contains(power.GetType()));

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

    /// <summary>
    /// Quita solo mejoras positivas marcadas como ofensivas. Fuerza positiva está registrada por
    /// defecto; Fuerza negativa conserva su semántica de Debuff y no se toca.
    /// </summary>
    public static async Task<int> RemoveOffensiveBuffs(
        Creature target, int max = int.MaxValue, Func<PowerModel, bool>? keep = null)
    {
        var toRemove = target.Powers
            .Where(p => IsOffensiveBuff(p) && (keep == null || !keep(p)))
            .Take(max).ToList();
        foreach (var power in toRemove) await PowerCmd.Remove(power);
        return toRemove.Count;
    }
}
