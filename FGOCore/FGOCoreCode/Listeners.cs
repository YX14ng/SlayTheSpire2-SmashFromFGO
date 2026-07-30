using System.Collections.Generic;
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
    {
        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is T match) yield return match;
        }
    }

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
    {
        foreach (var listener in PowersOf<T>(creature)) yield return listener;
        foreach (var listener in RelicsOf<T>(creature)) yield return listener;
    }

    /// <summary>True si algún power o reliquia del dueño implementa <typeparamref name="T"/>.</summary>
    public static bool Any<T>(Creature creature) where T : class
    {
        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is T) return true;
        }
        if (creature.Player?.Relics is not { } relics) return false;
        foreach (var relic in relics)
        {
            if (relic is T) return true;
        }
        return false;
    }

    /// <summary>El primero (power o reliquia) que implementa <typeparamref name="T"/>, o null.</summary>
    public static T? First<T>(Creature creature) where T : class
    {
        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is T match) return match;
        }
        if (creature.Player?.Relics is not { } relics) return null;
        foreach (var relic in relics)
        {
            if (relic is T match) return match;
        }
        return null;
    }

    /// <summary>
    /// Notifica de forma asíncrona y EN ORDEN a cada listener <typeparamref name="T"/> del dueño
    /// (powers, luego reliquias). Es el patrón exacto de <c>Lahmu.Devour</c> al avisar el devorar.
    /// </summary>
    public static async Task ForEachListener<T>(Creature creature, Func<T, Task> action) where T : class
    {
        // .ToList(): un listener puede aplicar/remover powers al reaccionar (muta la colección en
        // iteración) → InvalidOperationException latente. Se materializa antes de notificar.
        var snapshot = new List<T>();
        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is T match) snapshot.Add(match);
        }
        if (creature.Player?.Relics is { } relics)
        {
            foreach (var relic in relics)
            {
                if (relic is T match) snapshot.Add(match);
            }
        }
        foreach (var listener in snapshot)
        {
            await action(listener);
        }
    }

    /// <summary>Variante síncrona de <see cref="ForEachListener{T}"/> (acumular cap/suma/flag).</summary>
    public static void ForEach<T>(Creature creature, Action<T> action) where T : class
    {
        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is T match) action(match);
        }
        if (creature.Player?.Relics is not { } relics) return;
        foreach (var relic in relics)
        {
            if (relic is T match) action(match);
        }
    }
}
