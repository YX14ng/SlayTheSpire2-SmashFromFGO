using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Helpers del recurso *Aliento. TODO cambio de Aliento pasa por acá para que los topes
/// (0..Cap) y el cap de 1 *Tos/turno por agotamiento se respeten siempre (espejo de NpCharge).
/// </summary>
public static class Aliento
{
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
            await PowerCmd.Apply<AlientoPower>(creature, Math.Min(amount, AlientoPower.Max), creature, source);
            return;
        }
        var room = Math.Max(0, power.Cap - power.Amount);
        var toAdd = Math.Min(amount, room);
        if (toAdd > 0) await PowerCmd.ModifyAmount(power, toAdd, creature, source);
    }

    /// <summary>
    /// Gasta Aliento por una Ráfaga. Si el gasto deja el Aliento en 0 y no se tocó 0 antes
    /// este turno, genera 1 *Tos al mazo de robo (cap 1/turno). No baja de 0.
    /// </summary>
    public static async Task Spend(Creature creature, int amount, CardModel? source)
    {
        if (amount <= 0) return;
        var power = Power(creature);
        if (power == null) return;

        var spent = Math.Min(amount, power.Amount);
        if (spent <= 0) return;

        if (spent >= power.Amount) await PowerCmd.Remove(power);
        else await PowerCmd.ModifyAmount(power, -spent, creature, source);

        // Llegar a 0 cuesta una Tos (cap 1/turno por agotamiento).
        if (Of(creature) <= 0 && power.Amount == 0)
        {
            if (!power.HitZeroThisTurn)
            {
                power.HitZeroThisTurn = true;
                await Tos.ShuffleIntoDraw(creature, source);
            }
        }
    }

    /// <summary>Llena el Aliento al tope actual (Cerezo en Plena Floración).</summary>
    public static async Task FillToCap(Creature creature, CardModel? source)
    {
        var power = Power(creature);
        if (power == null)
        {
            await PowerCmd.Apply<AlientoPower>(creature, AlientoPower.StartingBreath, creature, source);
            return;
        }
        var room = Math.Max(0, power.Cap - power.Amount);
        if (room > 0) await PowerCmd.ModifyAmount(power, room, creature, source);
    }
}
