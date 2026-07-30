using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Curses;

/// <summary>
/// Helpers for the Maldición keyword. ALL curse changes go through here so the
/// per-enemy cap (<see cref="CursePower.MaxPerEnemy"/>) and the amplifiers
/// (<see cref="ICurseAmplifier"/>) hold everywhere.
/// </summary>
public static class Curses
{
    public static int Of(Creature creature) => creature.GetPowerAmount<CursePower>();

    /// <summary>Apply curse from <paramref name="applier"/>, adding its amplifiers and
    /// respecting the per-enemy cap. Returns the amount actually applied.</summary>
    public static Task<int> Apply(Creature target, int amount, Creature? applier, CardModel? cardSource) =>
        Apply(new BlockingPlayerChoiceContext(), target, amount, applier, cardSource);

    public static async Task<int> Apply(
        PlayerChoiceContext choiceContext,
        Creature target,
        int amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (target.IsDead || amount <= 0) return 0;

        // Regla UNICA (audit 2026-07-05): ICurseAmplifier amplifica SOLO aplicaciones con carta
        // (cardSource != null) — es lo que su texto promete ("tus cartas que aplican Maldicion").
        // Los ticks automaticos (lluvias de forma, Marea Corrosiva, Ojos Feericos, etc.) aplican el
        // valor plano; antes dependia de que applier pasara cada caller y quedaba inconsistente.
        if (applier != null && cardSource != null)
        {
            foreach (var power in applier.GetPowerInstances<PowerModel>())
            {
                if (power is ICurseAmplifier amp) amount += amp.ExtraCurse;
            }
        }

        var toAdd = Math.Min(amount, CursePower.MaxPerEnemy - Of(target));
        if (toAdd <= 0) return 0;

        await PowerCmd.Apply<CursePower>(choiceContext, target, toAdd, applier, cardSource);
        return toAdd;
    }

    /// <summary>Impuesto (存在税): consume up to <paramref name="upTo"/> Curse from the
    /// target (forfeiting its deferred damage). Returns the amount consumed.</summary>
    public static Task<int> Consume(Creature target, int upTo) =>
        Consume(new BlockingPlayerChoiceContext(), target, upTo);

    public static async Task<int> Consume(
        PlayerChoiceContext choiceContext,
        Creature target,
        int upTo)
    {
        var power = target.GetPower<CursePower>();
        if (power == null || upTo <= 0) return 0;

        var consumed = Math.Min(upTo, power.Amount);
        if (consumed >= power.Amount)
        {
            await PowerCmd.Remove(power);
        }
        else
        {
            await PowerCmd.ModifyAmount(choiceContext, power, -consumed, target, null);
        }
        return consumed;
    }

    /// <summary>The living enemy with the highest Curse, or null if none is cursed.</summary>
    public static Creature? MostCursed(CombatState combatState, Creature ofPlayer)
    {
        Creature? best = null;
        var bestAmount = 0;
        foreach (var enemy in combatState.GetOpponentsOf(ofPlayer))
        {
            if (enemy.IsDead) continue;
            var amount = Of(enemy);
            if (amount > bestAmount)
            {
                bestAmount = amount;
                best = enemy;
            }
        }
        return best;
    }

    public static Creature? MostCursed(Creature ofPlayer)
        => ofPlayer.CombatState is CombatState combatState
            ? MostCursed(combatState, ofPlayer)
            : null;

    /// <summary>How many living enemies are cursed right now.</summary>
    public static int CursedEnemies(CombatState combatState, Creature ofPlayer)
    {
        var count = 0;
        foreach (var enemy in combatState.GetOpponentsOf(ofPlayer))
        {
            if (!enemy.IsDead && Of(enemy) > 0) count++;
        }
        return count;
    }

    public static int CursedEnemies(Creature ofPlayer)
        => ofPlayer.CombatState is CombatState combatState
            ? CursedEnemies(combatState, ofPlayer)
            : 0;
}
