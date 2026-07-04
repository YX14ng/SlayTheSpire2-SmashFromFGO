using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

        // Preventers VANILLA como fuentes (audit 2026-07-04): el juego elige UN solo preventer. Si el
        // elegido es el nuestro (Bulwark/forma), Enforce recortaba bloqueo que Barricade/Blur/Burrowed
        // (retienen todo) o la Sturdy Clamp (retiene 10) garantizaban — y viceversa. Incorporarlos acá
        // hace el resultado idéntico gane quien gane la carrera de preventers (la promesa de esta clase).
        if (creature.GetPower<MegaCrit.Sts2.Core.Models.Powers.BarricadePower>() != null
            || creature.GetPower<MegaCrit.Sts2.Core.Models.Powers.BlurPower>() != null
            || creature.GetPower<MegaCrit.Sts2.Core.Models.Powers.BurrowedPower>() != null)
        {
            return decimal.MaxValue;
        }
        if (creature.Player != null)
        {
            foreach (var relic in creature.Player.Relics)
            {
                if (relic is MegaCrit.Sts2.Core.Models.Relics.SturdyClamp && best < 10m) best = 10m;
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

    /// <summary>Gain Block that persists between turns (Bulwark): block + matching Bulwark stacks.
    /// <paramref name="choiceContext"/>: si el caller corre en un hook de borde de turno, pasá el
    /// contexto del hook (sincronizado); un contexto fresco solo para caminos fuera de hooks.</summary>
    public static async Task GainBulwarkBlock(CardModel card, Creature creature, BlockVar blockVar, CardPlay? cardPlay, PlayerChoiceContext? choiceContext = null)
    {
        var gained = await CreatureCmd.GainBlock(creature, blockVar, cardPlay);
        if (gained > 0)
        {
            await PowerCmd.Apply<BulwarkPower>(choiceContext ?? new BlockingPlayerChoiceContext(), creature, gained, creature, card);
        }
    }

    /// <summary>Bulwark Block from a flat amount (powers, relics). Ver nota de contexto arriba.</summary>
    public static async Task GainBulwarkBlock(CardModel? source, Creature creature, decimal amount, ValueProp props = ValueProp.Move, PlayerChoiceContext? choiceContext = null)
    {
        var gained = await CreatureCmd.GainBlock(creature, amount, props, null);
        if (gained > 0)
        {
            await PowerCmd.Apply<BulwarkPower>(choiceContext ?? new BlockingPlayerChoiceContext(), creature, gained, creature, source);
        }
    }
}
