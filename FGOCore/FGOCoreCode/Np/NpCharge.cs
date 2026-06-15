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
            // Invoke() de un delegado multicast solo DEVUELVE la Task del último handler:
            // los demás corrían sin await (fire-and-forget). Esperamos todos en orden.
            foreach (var handler in GaugeFilled.GetInvocationList().Cast<Func<Creature, Task>>())
            {
                await handler(creature);
            }
        }
    }

    // Parche P3 del panel de rediseño: los waivers NUNCA cubren las ultis
    // auto-manifestadas (CardRarity.Event) — solo las cartas NP manuales del pool.
    private static INpCostWaiver? GetWaiver(Creature creature, CardModel? source)
    {
        if (source != null && source.Rarity == MegaCrit.Sts2.Core.Entities.Cards.CardRarity.Event) return null;
        return creature.GetPowerInstances<PowerModel>().OfType<INpCostWaiver>().FirstOrDefault(w => !w.Used);
    }

    /// <summary>Can an NP card costing <paramref name="amount"/> be played right now?</summary>
    public static bool CanPay(Creature creature, int amount, CardModel? source = null) =>
        Current(creature) >= amount || GetWaiver(creature, source) != null;

    /// <summary>
    /// True when the gauge reached the 100 threshold — NP cards played now trigger their
    /// peak effects. Check BEFORE spending.
    /// </summary>
    public static bool IsOvercharged(Creature creature) => Current(creature) >= NpChargePower.ManifestThreshold;

    /// <summary>Pay an NP card's charge cost. A waiver power covers it for free.</summary>
    public static async Task PayForNpCard(Creature creature, int amount, CardModel source)
    {
        var waiver = GetWaiver(creature, source);
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
    /// (>= minCost). Parche P3: con waiver el NP sale gratis pero resuelve a tier
    /// MÍNIMO (sin doble-dip de banco lleno + medidor intacto).
    /// </summary>
    public static async Task<int> ConsumeAllForNpCard(Creature creature, int minCost, CardModel source)
    {
        var current = Current(creature);
        var waiver = GetWaiver(creature, source);
        var tier = waiver != null ? minCost : Math.Max(current, minCost);

        // Bendición de Rhongomyniad: +10 al tier de Sobrecarga por carga, se consume.
        var blessing = creature.GetPower<OverchargeBlessingPower>();
        if (blessing != null)
        {
            tier += blessing.Amount * OverchargeBlessingPower.TierPerStack;
            await PowerCmd.Remove(blessing);
        }

        if (waiver != null)
        {
            waiver.Used = true;
        }
        else if (current > 0)
        {
            await Spend(creature, current, source);
        }

        // P5: marca que UNA carta-NP resolvió este turno (último paso, tras consumir
        // OverchargeBlessing y el medidor). Único punto por el que pasan TODAS las cartas-NP,
        // así que el flag cubre cualquier ult del ecosistema sin duplicar el set por carta.
        await MarkNpResolvedThisTurn(creature);
        return tier;
    }

    /// <summary>True if an NP card already resolved this turn (P5). Capture this BEFORE
    /// calling <see cref="ConsumeAllForNpCard"/> (which sets it) to size a refund.</summary>
    public static bool WasNpResolvedThisTurn(Creature creature) => creature.HasPower<NpResolvedThisTurnPower>();

    /// <summary>Set the once-per-turn "an NP card resolved" marker (auto-removed next turn).</summary>
    public static async Task MarkNpResolvedThisTurn(Creature creature)
    {
        if (!creature.HasPower<NpResolvedThisTurnPower>())
        {
            await PowerCmd.Apply<NpResolvedThisTurnPower>(creature, 1m, creature, null);
        }
    }

    /// <summary>Refund NP after an NP card: <paramref name="full"/> if this was the first NP
    /// card of the turn, else <paramref name="reduced"/> (P5 anti-double-refund). Pass the
    /// value of <see cref="WasNpResolvedThisTurn"/> captured BEFORE the consume.</summary>
    public static async Task RefundAfterNpCard(Creature creature, int full, int reduced, bool wasAlreadyResolvedBeforeThisCard, CardModel? source)
    {
        await Gain(creature, wasAlreadyResolvedBeforeThisCard ? reduced : full, source);
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
            foreach (var handler in GaugeDropped.GetInvocationList().Cast<Func<Creature, Task>>())
            {
                await handler(creature);
            }
        }
        return true;
    }
}
