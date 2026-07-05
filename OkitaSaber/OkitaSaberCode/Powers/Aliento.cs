using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Helpers del recurso *Aliento. TODO cambio de Aliento pasa por acá para que los topes
/// (0..Cap) y el cap de 1 *Tos/turno por agotamiento se respeten siempre (espejo de NpCharge).
/// </summary>
public static class Aliento
{
    // "Toco 0 este turno" vive FUERA del power (audit 2026-07-04): el power se REMUEVE al llegar a 0
    // y el flag moria con el — el cap de 1 Tos/turno se rompia y GudagudaPoster perdia la senal.
    // Per-creature, per-cliente (flujo sincronizado); lo resetea el Haori al inicio de turno/combate.
    // ConditionalWeakTable (audit 2026-07-05): el HashSet estatico retenia cada Creature (y su grafo
    // de run) PARA SIEMPRE entre combates/runs. Las claves debiles se liberan con la criatura.
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Creature, object> HitZero = new();

    public static bool HitZeroThisTurn(Creature creature) => HitZero.TryGetValue(creature, out _);

    internal static void ResetHitZero(Creature creature) => HitZero.Remove(creature);

    public static AlientoPower? Power(Creature creature) => creature.GetPower<AlientoPower>();

    public static int Of(Creature creature) => creature.GetPowerAmount<AlientoPower>();

    /// <summary>¿Puede pagar un coste de Aliento (una *Ráfaga) ahora mismo?</summary>
    public static bool CanPay(Creature creature, int cost) => Of(creature) >= cost;

    /// <summary>Gana Aliento, capeado al tope actual del power (Cap).</summary>
    public static async Task Gain(Creature creature, int amount, CardModel? source)
    {
        if (amount <= 0) return;
        var power = Power(creature);
        if (power == null)
        {
            await PowerCmd.Apply<AlientoPower>(new BlockingPlayerChoiceContext(), creature, Math.Min(amount, AlientoPower.Max), creature, source);
            return;
        }
        var room = Math.Max(0, power.Cap - power.Amount);
        var toAdd = Math.Min(amount, room);
        if (toAdd > 0) await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, toAdd, creature, source);
    }

    /// <summary>
    /// Gasta Aliento por una Ráfaga. Si el gasto deja el Aliento en 0 y no se tocó 0 antes
    /// este turno, genera 1 *Tos al mazo de robo (cap 1/turno). No baja de 0.
    /// </summary>
    /// <param name="grantTosOnEmpty">false para gastos que YA pagan su propio costo (p.ej. el boost
    /// del NP, que en la rama sin aire da su propia Tos — evita el doble castigo either/or).</param>
    public static async Task Spend(Creature creature, int amount, CardModel? source, bool grantTosOnEmpty = true)
    {
        if (amount <= 0) return;
        var power = Power(creature);
        if (power == null) return;

        var spent = Math.Min(amount, power.Amount);
        if (spent <= 0) return;

        var emptied = spent >= power.Amount;
        var alreadyHitZero = HitZero.TryGetValue(creature, out _);
        if (emptied) HitZero.AddOrUpdate(creature, new object());

        if (emptied) await PowerCmd.Remove(power);
        else await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, -spent, creature, source);

        // Llegar a 0 cuesta una Tos (cap 1/turno por agotamiento).
        if (grantTosOnEmpty && emptied && !alreadyHitZero)
            await Tos.ShuffleIntoDraw(creature, source);
    }

    /// <summary>Llena el Aliento al tope actual (Cerezo en Plena Floración).</summary>
    public static async Task FillToCap(Creature creature, CardModel? source)
    {
        var power = Power(creature);
        if (power == null)
        {
            // Con Aliento en 0 el power NO existe: aplicarlo y SEGUIR llenando hasta el Cap real
            // (audit 2026-07-04: antes se quedaba en 6 justo en el caso de uso principal de
            // Cerezo en Plena Floracion — recuperarse del agotamiento).
            await PowerCmd.Apply<AlientoPower>(new BlockingPlayerChoiceContext(), creature, AlientoPower.StartingBreath, creature, source);
            power = Power(creature);
            if (power == null) return;
        }
        var room = Math.Max(0, power.Cap - power.Amount);
        if (room > 0) await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, room, creature, source);
    }
}
