using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using FGOCore.FGOCoreCode.Cleanse;
using OberonPretender.OberonPretenderCode.Powers.Sleep;

namespace OberonPretender.OberonPretenderCode.Extensions;

/// <summary>
/// Helpers del mod Oberon. Centraliza las operaciones de masa de las Desatadas / cartas-NP y el
/// cleanse permitido por la regla P8. Sin APIs nuevas: todo reusa PowerCmd / la economía de Sueño.
/// </summary>
public static class OberonExtensions
{
    /// <summary>
    /// Quita SOLO los Debuff efectivos del propio jugador, delegando en la API compartida
    /// <see cref="Cleanse.RemoveDebuffs"/> de FGOCore. Los RECURSOS (NP/Estrellas/Sobrecarga + la Deuda
    /// de Oberon, que es Debuff-type pero implementa <see cref="IResourcePower"/>) NO se tocan;
    /// Maldición/Débil/Vulnerable sí se limpian (deseado). Lo usa «Solo un Sueño».
    /// </summary>
    public static Task RemoveAllDebuffs(Creature target, int max = int.MaxValue)
        => Cleanse.RemoveDebuffs(target, max);

    /// <summary>
    /// Remové la Fuerza POSITIVA de todos los enemigos vivos (el strip del NP real de Rye Rhyme
    /// Goodfellow). Lectura pura + PowerCmd.ModifyAmount negativo (patrón Cadenas del Cielo): la Fuerza
    /// negativa (un debuff que les pusiste) queda intacta. NO instala nada.
    /// </summary>
    public static async Task StripPositiveStrengthFromAll(CombatState combatState, Creature ofPlayer)
    {
        foreach (var enemy in combatState.GetOpponentsOf(ofPlayer).ToList())
        {
            if (enemy.IsDead) continue;
            var strength = enemy.GetPower<StrengthPower>();
            if (strength == null || strength.Amount <= 0) continue;
            await PowerCmd.ModifyAmount(strength, -strength.Amount, ofPlayer, null);
        }
    }

    /// <summary>
    /// Duerme a todos los enemigos vivos (vía <see cref="Sleep.TryApply"/>, que respeta Insomnio): el
    /// sueño masivo de la Desatada en sobrecarga ≥150. Devuelve cuántos durmió.
    /// </summary>
    public static async Task<int> SleepAll(CombatState combatState, Creature ofPlayer, int duration, CardModel? source)
    {
        var slept = 0;
        foreach (var enemy in combatState.GetOpponentsOf(ofPlayer).ToList())
        {
            if (await Sleep.TryApply(enemy, duration, ofPlayer, source)) slept++;
        }
        return slept;
    }
}
