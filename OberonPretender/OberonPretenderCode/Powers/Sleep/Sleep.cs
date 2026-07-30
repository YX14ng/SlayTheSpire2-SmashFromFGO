using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers.Sleep;

/// <summary>
/// Helper de la economia de Sueno (espejo de Curses/Lahmu). TODA aplicacion de Sueno pasa por aca
/// para respetar el candado de Insomnio (no re-dormible) y para enganchar el "skip de accion" con
/// <c>CreatureCmd.Stun</c> de forma uniforme.
/// </summary>
public static class Sleep
{
    public const int DefaultDuration = 1;

    public static bool IsAsleep(Creature creature) => creature.HasPower<SleepPower>();

    public static bool HasInsomnia(Creature creature) => creature.HasPower<InsomniaPower>();

    /// <summary>Cantidad de enemigos vivos Dormidos (lo leen Sueno Ligero, Mientras el Mundo
    /// Duerme, etc.).</summary>
    public static int SleepingEnemies(ICombatState combatState, Creature ofPlayer)
    {
        var count = 0;
        foreach (var enemy in combatState.GetOpponentsOf(ofPlayer))
        {
            if (!enemy.IsDead && IsAsleep(enemy)) count++;
        }
        return count;
    }

    /// <summary>Version segura para glows y previews, que tambien se evaluan fuera de combate.</summary>
    public static int SleepingEnemies(Creature ofPlayer) =>
        ofPlayer.CombatState is { } combatState ? SleepingEnemies(combatState, ofPlayer) : 0;

    /// <summary>
    /// Duerme al objetivo: salta su proxima accion (Stun) y lo vuelve intocable hasta el final de su
    /// turno (SleepPower). NO duerme si tiene Insomnio o ya esta dormido. Devuelve true si durmio.
    /// </summary>
    public static Task<bool> TryApply(
        Creature target,
        int duration,
        Creature? applier,
        MegaCrit.Sts2.Core.Models.CardModel? source) =>
        TryApply(new BlockingPlayerChoiceContext(), target, duration, applier, source);

    public static async Task<bool> TryApply(
        PlayerChoiceContext choiceContext,
        Creature target,
        int duration,
        Creature? applier,
        MegaCrit.Sts2.Core.Models.CardModel? source)
    {
        if (target.IsDead || duration <= 0) return false;
        if (HasInsomnia(target) || IsAsleep(target)) return false;

        await CreatureCmd.Stun(target);
        await PowerCmd.Apply<SleepPower>(choiceContext, target, duration, applier, source);
        return true;
    }
}
