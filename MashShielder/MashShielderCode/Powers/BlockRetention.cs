using MashShielder.MashShielderCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Single source of truth for how much Block Mash retains at the start of her turn.
/// The game picks ONE block-clear preventer, so every preventer (relic, Bulwark power)
/// delegates here and the result is identical no matter which one runs.
/// </summary>
public static class BlockRetention
{
    public static int Cap(Creature creature)
    {
        if (creature.HasPower<DistantUtopiaCastlePower>()) return int.MaxValue;

        var relicCap = 0;
        if (creature.Player?.GetRelic<LordCamelotRestored>() != null)
        {
            relicCap = LordCamelotRestored.MaxRetainedBlock;
        }
        else if (creature.Player?.GetRelic<RoundTableFragment>() != null)
        {
            relicCap = RoundTableFragment.MaxRetainedBlock;
        }
        return creature.GetPowerAmount<BulwarkPower>() + relicCap;
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
