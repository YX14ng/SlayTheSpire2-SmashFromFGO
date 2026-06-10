using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Helpers for the NP Charge resource. All charge changes must go through here
/// so the 0-100 bounds hold everywhere.
/// </summary>
public static class NpCharge
{
    public static int Current(Creature creature) => creature.GetPowerAmount<NpChargePower>();

    public static bool IsFull(Creature creature) => Current(creature) >= NpChargePower.Max;

    /// <summary>Gain charge, capped at 100.</summary>
    public static async Task Gain(Creature creature, int amount, CardModel? source)
    {
        var toAdd = Math.Min(amount, NpChargePower.Max - Current(creature));
        if (toAdd > 0)
        {
            await PowerCmd.Apply<NpChargePower>(creature, toAdd, creature, source);
        }
    }

    /// <summary>Can an NP card costing <paramref name="amount"/> be played right now?</summary>
    public static bool CanPay(Creature creature, int amount)
    {
        if (Current(creature) >= amount) return true;
        var pioneer = creature.GetPower<PioneerOfTheStarsPower>();
        return pioneer is { Used: false };
    }

    /// <summary>
    /// True when the gauge is exactly full — NP cards played now trigger their Overcharge effect.
    /// Check BEFORE spending.
    /// </summary>
    public static bool IsOvercharged(Creature creature) => Current(creature) >= NpChargePower.Max;

    /// <summary>
    /// Pay an NP card's charge cost. Pioneer of the Stars covers the first NP card each combat.
    /// </summary>
    public static async Task PayForNpCard(Creature creature, int amount, CardModel source)
    {
        var pioneer = creature.GetPower<PioneerOfTheStarsPower>();
        if (pioneer is { Used: false })
        {
            pioneer.Used = true;
            return;
        }
        await Spend(creature, amount, source);
    }

    /// <summary>Spend charge. Returns false (and spends nothing) if there isn't enough.</summary>
    public static async Task<bool> Spend(Creature creature, int amount, CardModel? source)
    {
        var power = creature.GetPower<NpChargePower>();
        if (power == null || power.Amount < amount) return false;

        if (power.Amount == amount)
        {
            await PowerCmd.Remove(power);
        }
        else
        {
            await PowerCmd.ModifyAmount(power, -amount, creature, source);
        }
        return true;
    }
}
