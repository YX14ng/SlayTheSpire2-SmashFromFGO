using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Block;

/// <summary>
/// Single source of truth for how much Block a creature retains at the start of its turn.
/// The game picks ONE block-clear preventer, so every preventer (relic, Bulwark power)
/// delegates here and the result is identical no matter which one runs.
/// Cap = Bulwark stacks + MAX over all <see cref="IBlockRetentionSource"/> (relics and powers).
/// </summary>
public static class BlockRetention
{
    public static decimal Cap(Creature creature)
    {
        decimal best = 0m;

        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is IBlockRetentionSource src)
            {
                var cap = src.RetentionCap(creature);
                if (cap == decimal.MaxValue) return decimal.MaxValue;
                if (cap > best) best = cap;
            }
        }

        if (creature.Player != null)
        {
            foreach (var relic in creature.Player.Relics)
            {
                if (relic is IBlockRetentionSource src)
                {
                    var cap = src.RetentionCap(creature);
                    if (cap == decimal.MaxValue) return decimal.MaxValue;
                    if (cap > best) best = cap;
                }
            }
        }

        return creature.GetPowerAmount<BulwarkPower>() + best;
    }

    /// <summary>Reduce current Block down to the retention cap (called after a prevented clear).</summary>
    public static async Task Enforce(Creature creature)
    {
        var block = creature.Block;
        if (block == 0) return;

        var cap = Cap(creature);
        if (block > cap)
        {
            await CreatureCmd.LoseBlock(creature, block - cap);
        }
    }

    /// <summary>Gain Block that persists between turns (Bulwark): block + matching Bulwark stacks.</summary>
    public static async Task GainBulwarkBlock(CardModel card, Creature creature, BlockVar blockVar, CardPlay? cardPlay)
    {
        var gained = await CreatureCmd.GainBlock(creature, blockVar, cardPlay);
        if (gained > 0)
        {
            await PowerCmd.Apply<BulwarkPower>(creature, gained, creature, card);
        }
    }

    /// <summary>Bulwark Block from a flat amount (powers, relics).</summary>
    public static async Task GainBulwarkBlock(CardModel? source, Creature creature, decimal amount, ValueProp props = ValueProp.Move)
    {
        var gained = await CreatureCmd.GainBlock(creature, amount, props, null);
        if (gained > 0)
        {
            await PowerCmd.Apply<BulwarkPower>(creature, gained, creature, source);
        }
    }
}
