using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode;

/// <summary>
/// Helpers para el patrón "recorrer los powers (y opcionalmente las reliquias) del dueño
/// buscando una interfaz <c>IFoo</c> y reaccionar". Se repetía a mano por todo FGOCore:
/// <c>Lahmu.Devour</c> (avisa a relics + powers que implementan <c>ILahmuDevourListener</c>),
/// <c>Curses.Apply</c> (suma de <c>ICurseAmplifier</c> en powers), <c>BlockRetention.Cap</c>
/// (máximo de <c>IBlockRetentionSource</c> en powers + relics), <c>GutsPower.Floor</c>
/// (<c>IGutsFloorBooster</c> en relics), <c>DragonScalesPower</c>
/// (<c>ISdDSuppressor</c> / <c>IDragonScalePiercer</c>), <c>NpCharge.GetWaiver</c>
/// (<c>INpCostWaiver</c> en powers), <c>NpLevels</c> (<c>INpLevelStore</c>/<c>ILimitBreaker</c>).
///
/// ADITIVO: nadie está obligado a migrar; estos métodos sólo ofrecen una forma única y testeada
/// de hacer el escaneo. <typeparamref name="T"/> es la INTERFAZ marcadora (no un PowerModel),
/// por eso no hay restricción <c>where T : PowerModel</c>: se filtra con <c>OfType&lt;T&gt;</c>.
/// </summary>
public static class Listeners
{
    /// <summary>Los powers del dueño que implementan <typeparamref name="T"/>.</summary>
    public static IEnumerable<T> PowersOf<T>(Creature creature) where T : class
        => creature.GetPowerInstances<PowerModel>().OfType<T>();

    /// <summary>Las reliquias del jugador dueño que implementan <typeparamref name="T"/> (vacío si no hay jugador).</summary>
    public static IEnumerable<T> RelicsOf<T>(Creature creature) where T : class
    {
        var relics = creature.Player?.Relics;
        if (relics == null) yield break;
        foreach (var relic in relics)
        {
            if (relic is T match) yield return match;
        }
    }

    /// <summary>
    /// Powers Y reliquias del dueño que implementan <typeparamref name="T"/> (powers primero,
    /// luego reliquias — el orden que usaba <c>Lahmu.Devour</c> y <c>BlockRetention.Cap</c>).
    /// </summary>
    public static IEnumerable<T> Of<T>(Creature creature) where T : class
        => PowersOf<T>(creature).Concat(RelicsOf<T>(creature));

    /// <summary>True si algún power o reliquia del dueño implementa <typeparamref name="T"/>.</summary>
    public static bool Any<T>(Creature creature) where T : class
        => PowersOf<T>(creature).Any() || RelicsOf<T>(creature).Any();

    /// <summary>El primero (power o reliquia) que implementa <typeparamref name="T"/>, o null.</summary>
    public static T? First<T>(Creature creature) where T : class
        => Of<T>(creature).FirstOrDefault();

    /// <summary>
    /// Notifica de forma asíncrona y EN ORDEN a cada listener <typeparamref name="T"/> del dueño
    /// (powers, luego reliquias). Es el patrón exacto de <c>Lahmu.Devour</c> al avisar el devorar.
    /// </summary>
    public static async Task ForEachListener<T>(Creature creature, Func<T, Task> action) where T : class
    {
        foreach (var listener in Of<T>(creature))
        {
            await action(listener);
        }
    }

    /// <summary>Variante síncrona de <see cref="ForEachListener{T}"/> (acumular cap/suma/flag).</summary>
    public static void ForEach<T>(Creature creature, Action<T> action) where T : class
    {
        foreach (var listener in Of<T>(creature))
        {
            action(listener);
        }
    }
}
