using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Combat;

/// <summary>
/// Helper de la VENTANA-NP (modelo de NP nuevo, 2026-06-12): cruzar 100 de Carga NP NO genera
/// una carta-ulti gratis, sino que abre una ventana de 1 turno que potencia el mazo y
/// DEVUELVE recursos (+1⚡ y robar 1, "arranca el turno grande, no lo reemplaza" — modelo
/// Phrolova). El paquete exacto y el power-ventana son POR-MOD; lo que se repetía idéntico en
/// los <c>MainFile.cs</c> de Mash / Artoria / Tiamat era:
///   1. aplicar el power-ventana, y
///   2. el cierre <c>GainEnergy(energy) + Draw(draw)</c> guardado por <c>Player != null</c>.
///
/// ADITIVO: el power-ventana se pasa como parámetro de tipo (es por-mod). Las llamadas siguen
/// gestionando su marcador y sus efectos propios (estrellas, Anti-Purga, cría, cambio de forma);
/// este helper sólo factoriza las dos piezas comunes. <see cref="ReturnResources"/> queda público
/// para los mods que quieran solo el cierre de recursos.
/// </summary>
public static class NpWindow
{
    /// <summary>
    /// Aplica el power-ventana <typeparamref name="TWindow"/> sobre <paramref name="creature"/> y,
    /// si tiene jugador, devuelve recursos (+<paramref name="energy"/>⚡, robar <paramref name="draw"/>).
    /// El marcador "ya se abrió este pico/combate" y los efectos extra los maneja el caller.
    /// </summary>
    public static async Task OpenWindow<TWindow>(Creature creature, int energy = 1, int draw = 1)
        where TWindow : PowerModel
    {
        await PowerCmd.Apply<TWindow>(creature, 1m, creature, null);
        await ReturnResources(creature, energy, draw);
    }

    /// <summary>
    /// Igual que <see cref="OpenWindow{TWindow}"/> pero además aplica el power-MARCADOR
    /// <typeparamref name="TMarker"/> ANTES del power-ventana (el orden que usaban Mash/Tiamat:
    /// marcador primero, ventana después). El caller sigue siendo dueño de chequear
    /// <c>HasPower&lt;TMarker&gt;()</c> y de re-armarlo en <c>GaugeDropped</c>.
    /// </summary>
    public static async Task OpenWindow<TWindow, TMarker>(Creature creature, int energy = 1, int draw = 1)
        where TWindow : PowerModel
        where TMarker : PowerModel
    {
        await PowerCmd.Apply<TMarker>(creature, 1m, creature, null);
        await PowerCmd.Apply<TWindow>(creature, 1m, creature, null);
        await ReturnResources(creature, energy, draw);
    }

    /// <summary>
    /// El cierre común de recursos: +<paramref name="energy"/> energía y robar <paramref name="draw"/>
    /// cartas, sólo si la criatura tiene jugador. No-op para montos &lt;= 0.
    /// </summary>
    public static async Task ReturnResources(Creature creature, int energy, int draw)
    {
        var player = creature.Player;
        if (player == null) return;

        if (energy > 0)
        {
            await PlayerCmd.GainEnergy(energy, player);
        }
        if (draw > 0)
        {
            await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), draw, player);
        }
    }
}
