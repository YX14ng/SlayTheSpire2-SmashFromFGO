using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Helper for the Critical Stars economy (mirrors FGOCore's Curses helper).
/// CRITICAL X★: an attack card may consume X stars to use its critical value —
/// ONLY in Berserker or Avalon form (the hard form gate of the design).
/// </summary>
public static class Stars
{
    public static int Of(Creature creature) => (int)creature.GetPowerAmount<CriticalStarsPower>();

    /// <summary>Gain stars, clamped to the cap (10). Excess is wasted by design.</summary>
    public static async Task Gain(Creature creature, int amount, CardModel? source)
    {
        var current = Of(creature);
        var room = CriticalStarsPower.Max - current;
        if (room <= 0 || amount <= 0) return;
        await PowerCmd.Apply<CriticalStarsPower>(creature, Math.Min(amount, room), creature, source);
    }

    /// <summary>Critical discount from relics/powers implementing <see cref="ICritDiscount"/> (min cost 1).</summary>
    public static int DiscountedCost(Creature creature, int cost)
    {
        foreach (var power in creature.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>())
        {
            if (power is ICritDiscount discount) cost -= discount.CritCostReduction;
        }
        return Math.Max(1, cost);
    }

    /// <summary>Flat bonus added to every critical value (powers + relics implementing <see cref="ICritDamageBoost"/>).</summary>
    public static int CritBonus(Creature creature)
    {
        var bonus = 0;
        foreach (var power in creature.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>())
        {
            if (power is ICritDamageBoost boost) bonus += boost.CritDamageBonus;
        }
        var relics = creature.Player?.Relics;
        if (relics != null)
        {
            foreach (var relic in relics)
            {
                if (relic is ICritDamageBoost boost) bonus += boost.CritDamageBonus;
            }
        }
        return bonus;
    }

    /// <summary>True if the owner can crit (Berserker/Avalon form, OR the Around Caliburn
    /// NP window is open) and has ≥cost stars.</summary>
    public static bool CanCrit(Creature creature, int cost)
    {
        if (!creature.HasPower<SummerBerserkerFormPower>()
            && !creature.HasPower<AvalonFormPower>()
            && !creature.HasPower<AroundCaliburnWindowPower>()) return false;
        return Of(creature) >= DiscountedCost(creature, cost);
    }

    /// <summary>Consume the stars for a critical. Call only after <see cref="CanCrit"/>.</summary>
    public static async Task ConsumeForCrit(Creature creature, int cost, CardModel? source)
    {
        var spent = DiscountedCost(creature, cost);
        var power = creature.GetPowerInstances<CriticalStarsPower>().FirstOrDefault();
        if (power == null) return;
        await PowerCmd.ModifyAmount(power, -spent, creature, source, silent: true);
        foreach (var p in creature.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>())
        {
            if (p is ICritListener listener) await listener.AfterCritConsumed(spent);
        }
        var relics = creature.Player?.Relics;
        if (relics == null) return;
        foreach (var relic in relics)
        {
            if (relic is ICritListener listener) await listener.AfterCritConsumed(spent);
        }
    }
}

/// <summary>A power/relic that reduces the star cost of criticals (Instinto de la Espada).</summary>
public interface ICritDiscount
{
    int CritCostReduction { get; }
}

/// <summary>A power/relic that adds flat damage to critical values (Magia Única, Lupa).</summary>
public interface ICritDamageBoost
{
    int CritDamageBonus { get; }
}

/// <summary>Reacts to a critical consuming stars (Lupa de la Detective, Magia Única).</summary>
public interface ICritListener
{
    Task AfterCritConsumed(int starsSpent);
}
