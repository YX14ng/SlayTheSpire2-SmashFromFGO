using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// Helpers for the NP Charge resource. All charge changes must go through here
/// so the 0-100 bounds hold everywhere. Character mods hook their ult-manifestation
/// (or other full-gauge effects) via <see cref="GaugeFilled"/>/<see cref="GaugeDropped"/>.
/// </summary>
public static class NpCharge
{
    /// <summary>Fired after a gain leaves the gauge at or above 100 (ManifestThreshold).
    /// Handlers decide per-creature (check the creature's character) and must be
    /// idempotent per peak.</summary>
    public static event Func<Creature, Task>? GaugeFilled;

    /// <summary>Fired after a spend leaves the gauge below 100 (re-arm point for ults).</summary>
    public static event Func<Creature, Task>? GaugeDropped;

    public static int Current(Creature creature) => creature.GetPowerAmount<NpChargePower>();

    public static bool IsFull(Creature creature) => Current(creature) >= NpChargePower.Max;

    /// <summary>Gain charge, capped at 300. Fires <see cref="GaugeFilled"/> when at or
    /// above the manifest threshold afterwards.</summary>
    public static async Task Gain(Creature creature, int amount, CardModel? source)
    {
        var toAdd = Math.Min(amount, NpChargePower.Max - Current(creature));
        if (toAdd > 0)
        {
            await PowerCmd.Apply<NpChargePower>(creature, toAdd, creature, source);
        }
        if (Current(creature) >= NpChargePower.ManifestThreshold && GaugeFilled != null)
        {
            await GaugeFilled.Invoke(creature);
        }
    }

    private static INpCostWaiver? GetWaiver(Creature creature) =>
        creature.GetPowerInstances<PowerModel>().OfType<INpCostWaiver>().FirstOrDefault(w => !w.Used);

    /// <summary>Can an NP card costing <paramref name="amount"/> be played right now?</summary>
    public static bool CanPay(Creature creature, int amount) =>
        Current(creature) >= amount || GetWaiver(creature) != null;

    /// <summary>
    /// True when the gauge reached the 100 threshold — NP cards played now trigger their
    /// peak effects. Check BEFORE spending.
    /// </summary>
    public static bool IsOvercharged(Creature creature) => Current(creature) >= NpChargePower.ManifestThreshold;

    /// <summary>Pay an NP card's charge cost. A waiver power covers it for free.</summary>
    public static async Task PayForNpCard(Creature creature, int amount, CardModel source)
    {
        var waiver = GetWaiver(creature);
        if (waiver != null)
        {
            waiver.Used = true;
            return;
        }
        await Spend(creature, amount, source);
    }

    /// <summary>
    /// FGO-style Overcharge: an NP card consumes ALL the gauge (at least its minimum cost)
    /// and its power scales with what was consumed. Returns the effective charge tier
    /// (>= minCost). A waiver makes the NP free without lowering the tier.
    /// </summary>
    public static async Task<int> ConsumeAllForNpCard(Creature creature, int minCost, CardModel source)
    {
        var current = Current(creature);
        var tier = Math.Max(current, minCost);

        // Bendición de Rhongomyniad: +10 al tier de Sobrecarga por carga, se consume.
        var blessing = creature.GetPower<OverchargeBlessingPower>();
        if (blessing != null)
        {
            tier += blessing.Amount * OverchargeBlessingPower.TierPerStack;
            await PowerCmd.Remove(blessing);
        }

        var waiver = GetWaiver(creature);
        if (waiver != null)
        {
            waiver.Used = true;
        }
        else if (current > 0)
        {
            await Spend(creature, current, source);
        }
        return tier;
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

        if (Current(creature) < NpChargePower.ManifestThreshold && GaugeDropped != null)
        {
            await GaugeDropped.Invoke(creature);
        }
        return true;
    }
}
