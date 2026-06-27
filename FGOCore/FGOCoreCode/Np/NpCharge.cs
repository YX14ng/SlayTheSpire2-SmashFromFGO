using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

    /// <summary>
    /// Dynamic charge ceiling gated by the owner's NP level (dupes). Un-duped (level 1)
    /// caps at <see cref="NpChargePower.ChargePerNpLevel"/> (=100, the manifest floor);
    /// each dupe lifts it by another <see cref="NpChargePower.ChargePerNpLevel"/>. The
    /// Holy Grail (<see cref="ILimitBreaker"/>, via <see cref="NpLevels.MaxLevel"/>)
    /// extends the hard cap above the base <see cref="NpChargePower.Max"/> (=300).
    /// Floored at <see cref="NpChargePower.ManifestThreshold"/> so ults can always manifest.
    /// </summary>
    public static int Max(Creature creature)
    {
        var player = creature.Player;
        if (player == null) return NpChargePower.ManifestThreshold;

        var level = NpLevels.Get(player);
        var raw = NpChargePower.ChargePerNpLevel * level;
        var hardCap = NpChargePower.Max
            + System.Math.Max(0, NpLevels.MaxLevel(player) - NpLevels.BaseMaxLevel) * NpChargePower.ChargePerNpLevel;
        return System.Math.Clamp(raw, NpChargePower.ManifestThreshold, hardCap);
    }

    public static bool IsFull(Creature creature) => Current(creature) >= Max(creature);

    // --- Guard de re-entrancia COMPARTIDO entre amplificadores de NP (GoldenRule, FormalCraft, ...) ---
    // Cada amplificador suma su extra con PowerCmd.ModifyAmount, que re-dispara AfterPowerAmountChanged en
    // TODOS los powers del dueño. Un guard per-INSTANCIA solo evita la re-entrada de sí mismo; este guard
    // per-CREATURE evita que un amplificador amplifique el bonus de OTRO (compounding cruzado). Estático
    // por-cliente, keyeado por Creature (no bloquea entre criaturas distintas en MP).
    private static readonly HashSet<Creature> AmplifyingCreatures = [];

    /// <summary>True mientras algún amplificador de NP está aplicando su extra sobre esta criatura.
    /// Los amplificadores deben checkearlo al entrar y saltar si es true.</summary>
    public static bool IsAmplifying(Creature creature) => AmplifyingCreatures.Contains(creature);

    /// <summary>Aplica un extra de Carga NP marcando el guard compartido, de modo que ningún otro
    /// amplificador reaccione a este ModifyAmount. Usalo en vez de un guard <c>_amplifying</c> local.</summary>
    public static async Task Amplify(PlayerChoiceContext choiceContext, NpChargePower np, int extra, Creature owner, CardModel? source)
    {
        AmplifyingCreatures.Add(owner);
        try
        {
            await PowerCmd.ModifyAmount(choiceContext, np, extra, owner, source);
        }
        finally
        {
            AmplifyingCreatures.Remove(owner);
        }
    }

    /// <summary>Gain charge, capped at 300. Fires <see cref="GaugeFilled"/> when at or
    /// above the manifest threshold afterwards.</summary>
    public static async Task Gain(Creature creature, int amount, CardModel? source)
    {
        var wasBelow = Current(creature) < NpChargePower.ManifestThreshold;
        var toAdd = Math.Min(amount, Max(creature) - Current(creature));
        if (toAdd > 0)
        {
            await PowerCmd.Apply<NpChargePower>(new BlockingPlayerChoiceContext(), creature, toAdd, creature, source);
        }
        // GaugeFilled SOLO en el CRUCE real del umbral (below -> >=100): no re-invocar los handlers de
        // los 9 personajes en cada gain con el medidor ya lleno (son no-op por sus markers, pero es
        // trabajo desperdiciado en camino caliente y un footgun para handlers futuros sin marker).
        if (toAdd > 0 && wasBelow && Current(creature) >= NpChargePower.ManifestThreshold && GaugeFilled != null)
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
            await PowerCmd.Apply<NpResolvedThisTurnPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);
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
        var wasAtOrAbove = Current(creature) >= NpChargePower.ManifestThreshold;

        if (power.Amount == amount)
        {
            await PowerCmd.Remove(power);
        }
        else
        {
            await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, -amount, creature, source);
        }

        // GaugeDropped SOLO en el CRUCE real (>=100 → below): no re-disparar si ya estabas debajo.
        if (wasAtOrAbove && Current(creature) < NpChargePower.ManifestThreshold && GaugeDropped != null)
        {
            foreach (var handler in GaugeDropped.GetInvocationList().Cast<Func<Creature, Task>>())
            {
                await handler(creature);
            }
        }
        return true;
    }
}
