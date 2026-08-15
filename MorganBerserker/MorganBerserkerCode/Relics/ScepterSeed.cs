using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// El motor «sangrar → sembrar» del cetro, extraído para que la Ancient
/// (<see cref="WorldsEndCoronation"/>) lo REINSTALE al reemplazar la starter (REDESIGN-MORGAN-V2
/// §6, parches J2-1/J3-4 — el hallazgo estructural del panel: tomar Orobas no puede amputar la
/// sembradora del arquetipo D ni el re-armado M3). Perder HP (cualquier fuente, salvo el tick de
/// FaeBloodPact) → 3 Maldición a un enemigo vivo aleatorio, cap 3 eventos/turno (bits 5-6 del
/// estado de turno — cetro y Ancient nunca coexisten, comparten el contador).
/// </summary>
public static class ScepterSeed
{
    public const int CursePerHpLoss = 3;
    public const int CurseTriggersPerTurn = 3;

    public static async Task OnHpLoss(
        RelicModel relic, PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, CardModel? cardSource)
    {
        var owner = relic.Owner;
        if (!CombatManager.Instance.IsInProgress || target != owner.Creature || result.UnblockedDamage <= 0) return;
        if (Powers.FaeBloodPactPower.TickInProgress) return; // P4: el tick del Pacto no siembra Maldición.

        if (owner.Creature.CombatState is not { } combatState) return;
        var living = new List<Creature>();
        foreach (var enemy in combatState.GetOpponentsOf(owner.Creature))
        {
            if (!enemy.IsDead) living.Add(enemy);
        }
        if (living.Count == 0) return;
        if (FgoCombatState.GetTurn(owner.Creature, 5, 2) >= CurseTriggersPerTurn) return;
        await FgoCombatState.IncrementTurn(
            choiceContext, owner.Creature, 5, CurseTriggersPerTurn, cardSource, width: 2);

        relic.Flash();
        var victim = living[owner.RunState.Rng.CombatCardGeneration.NextInt(living.Count)];
        // applier = Owner.Creature para que los amplificadores de Maldición (Caster/Invierno) cuenten.
        await Curses.Apply(choiceContext, victim, CursePerHpLoss, owner.Creature, null);
    }
}
