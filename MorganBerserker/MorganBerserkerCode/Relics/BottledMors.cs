using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Mors Embotellado (瓶中的摩耳斯) — when an enemy dies with Curse, its Curse jumps
/// to a random living enemy. The curse does not die with the debtor.
/// </summary>
public sealed class BottledMors : MorganRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeDeath(Creature creature)
    {
        if (creature.IsPlayer) return;

        var curse = Curses.Of(creature);
        if (curse <= 0) return;

        var living = new List<Creature>();
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead && enemy != creature) living.Add(enemy);
        }
        if (living.Count == 0) return;

        Flash();
        var target = living[Owner.RunState.Rng.CombatCardGeneration.NextInt(living.Count)];
        await Curses.Apply(target, curse, null, null);
    }
}
