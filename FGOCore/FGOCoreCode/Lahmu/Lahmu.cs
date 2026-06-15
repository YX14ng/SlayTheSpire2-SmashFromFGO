using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Lahmu;

/// <summary>
/// Helper de la economía del enjambre de Laḫmu (espejo de <c>Curses</c>/<c>NpCharge</c>).
/// PARIR sube el nº de larvas (tope 6), ALIMENTAR sube la Crianza global, DEVORAR sacrifica
/// larvas para un pico inmediato (el caller calcula el daño con el nº devuelto y la Crianza).
/// </summary>
public static class Lahmu
{
    public static int Count(Creature creature) => creature.GetPowerAmount<LahmuSwarmPower>();

    public static int NurtureOf(Creature creature) => creature.GetPowerAmount<LahmuNurturePower>();

    /// <summary>Parí <paramref name="n"/> Laḫmu (tope <see cref="LahmuSwarmPower.MaxSwarm"/>). Devuelve los realmente paridos.</summary>
    public static async Task<int> Spawn(Creature creature, int n, CardModel? source)
    {
        if (n <= 0) return 0;
        var room = LahmuSwarmPower.MaxSwarm - Count(creature);
        var toAdd = Math.Min(n, room);
        if (toAdd <= 0) return 0;
        await PowerCmd.Apply<LahmuSwarmPower>(creature, toAdd, creature, source);
        return toAdd;
    }

    /// <summary>Sube la Crianza global en <paramref name="n"/>.</summary>
    public static async Task Feed(Creature creature, int n, CardModel? source)
    {
        if (n <= 0) return;
        await PowerCmd.Apply<LahmuNurturePower>(creature, n, creature, source);
    }

    /// <summary>Sacrifica hasta <paramref name="n"/> Laḫmu. Devuelve cuántos se devoraron (para escalar el burst).</summary>
    public static async Task<int> Devour(Creature creature, int n)
    {
        var power = creature.GetPower<LahmuSwarmPower>();
        if (power == null || n <= 0) return 0;
        var eaten = Math.Min(n, power.Amount);
        if (eaten >= power.Amount)
        {
            await PowerCmd.Remove(power);
        }
        else
        {
            await PowerCmd.ModifyAmount(power, -eaten, creature, null);
        }
        // Avisar a los que escuchan el devorar (reliquias Y powers — la interfaz no es solo de
        // reliquias). Se hace tras consumir las larvas para que reflejen el estado ya actualizado.
        if (eaten > 0)
        {
            if (creature.Player?.Relics is { } relics)
            {
                foreach (var relic in relics)
                {
                    if (relic is ILahmuDevourListener relicListener)
                    {
                        await relicListener.OnLahmuDevoured(creature, eaten);
                    }
                }
            }
            foreach (var powerListener in creature.GetPowerInstances<PowerModel>().OfType<ILahmuDevourListener>())
            {
                await powerListener.OnLahmuDevoured(creature, eaten);
            }
        }
        return eaten;
    }

    /// <summary>150 si la dueña tiene un <see cref="IDevourAmplifier"/> (forma Bestia: Devorar
    /// +50%), si no 100. Las cartas que devoran multiplican su daño por este valor / 100.</summary>
    public static int DevourBonusMultiplierPct(Creature creature) =>
        creature.GetPowerInstances<PowerModel>().OfType<IDevourAmplifier>().Any() ? 150 : 100;
}
