using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Tesoro (财宝) — el «Oro de combate» de Gilgamesh (fallback del módulo Gold de FGOCore, que aún no
/// existe — DESIGN-GILGAMESH §10). Es un contador MOD-LOCAL, NO el oro de la run: gastar el oro real
/// en mitad del combate es estructuralmente arriesgado (afecta la economía meta), así que el «Pagá X
/// Oro» de los riders del diseño cobra de ESTE medidor, sembrado al iniciar combate por Bab-ilu.
///
/// Patrón <see cref="OberonPretender.OberonPretenderCode.Powers.DebtPower"/> de Oberon: contador
/// visible con API estática <see cref="Of"/>/<see cref="Add"/>/<see cref="TrySpend"/>. Personal
/// (no escala en MP). A diferencia de la Deuda, el Tesoro NO se cobra a fin de turno: es puro recurso.
///
/// Lo siembra <c>BabIlu.BeforeCombatStartLate</c> (<see cref="Seed"/>) y lo gastan las cartas
/// «Inversión Real», «Soborno Imposible», «Babilonia, Capital del Oro», «Lluvia de Tesoros».
/// </summary>
public sealed class TreasurePower : GilgameshPower
{
    /// <summary>Tesoro inicial sembrado al abrir combate (Bab-ilu). Tope sano para que no infle ad infinitum.</summary>
    public const int StartingAmount = 6;
    public const int Max = 30;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>Tesoro disponible (0 si nunca se sembró).</summary>
    public static int Of(Creature creature) => creature.GetPowerAmount<TreasurePower>();

    /// <summary>Suma Tesoro respetando el cap. Toda ganancia de Tesoro pasa por acá.</summary>
    public static async Task Add(Creature creature, int amount, Creature? applier, CardModel? source)
    {
        if (amount <= 0) return;
        var toAdd = Math.Min(amount, Max - Of(creature));
        if (toAdd <= 0) return;
        await PowerCmd.Apply<TreasurePower>(new BlockingPlayerChoiceContext(), creature, toAdd, applier, source);
    }

    /// <summary>Siembra el Tesoro de apertura una sola vez por combate (lo llama Bab-ilu).</summary>
    public static Task Seed(Creature creature, int amount) => Add(creature, amount, creature, null);

    /// <summary>¿Hay al menos <paramref name="cost"/> de Tesoro para pagar?</summary>
    public static bool CanAfford(Creature creature, int cost) => Of(creature) >= cost;

    /// <summary>Intenta gastar <paramref name="cost"/> de Tesoro. Devuelve true si pagó (y lo descontó);
    /// false si no alcanzaba (no descuenta nada — el rider chequea CanAfford en IsPlayable antes).</summary>
    public static async Task<bool> TrySpend(Creature creature, int cost)
    {
        if (cost <= 0) return true;
        var power = creature.GetPower<TreasurePower>();
        if (power == null || power.Amount < cost) return false;
        power.Flash();
        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, -cost, creature, null);
        return true;
    }
}
