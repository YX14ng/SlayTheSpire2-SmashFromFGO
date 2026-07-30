using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Seal;

/// <summary>
/// Resolver compartido del Sello de Habilidad de FGO. El modelo concreto puede pertenecer a
/// FGOCore o a un mod ya publicado: <see cref="Apply{TPower}"/> conserva el ID del power elegido.
/// </summary>
public static class SkillSeal
{
    public const int DefaultDuration = 1;

    /// <summary>Lectura pura de la intención visible. Los ataques y un STUNNED ya resuelto pasan.</summary>
    public static bool IntendsToUseSkill(Creature creature)
    {
        if (creature.Monster == null || creature.IsDead || creature.IsStunned) return false;
        return !creature.Monster.IntendsToAttack;
    }

    public static Task<bool> Apply(
        Creature target, int duration, Creature? applier, CardModel? source) =>
        Apply(new BlockingPlayerChoiceContext(), target, duration, applier, source);

    public static Task<bool> Apply(
        PlayerChoiceContext context, Creature target, int duration, Creature? applier, CardModel? source) =>
        Apply<SkillSealPower>(context, target, duration, applier, source);

    /// <summary>Aplica el modelo indicado y cancela inmediatamente una habilidad ya intencionada.</summary>
    public static async Task<bool> Apply<TPower>(
        PlayerChoiceContext context, Creature target, int duration, Creature? applier, CardModel? source)
        where TPower : PowerModel
    {
        if (target.IsDead || duration <= 0) return false;

        await PowerCmd.Apply<TPower>(context, target, duration, applier, source);
        if (IntendsToUseSkill(target)) await CreatureCmd.Stun(target);
        return true;
    }

    /// <summary>
    /// Timing compartido para modelos concretos: si el dueño participa, cancela su intención
    /// no-Ataque y consume una carga. El filtro por participantes evita tocar otra criatura/lado.
    /// </summary>
    public static async Task ResolveTurn(
        PowerModel power,
        PlayerChoiceContext context,
        IReadOnlyList<Creature> participants,
        ICombatState combatState,
        Action? onCancel = null)
    {
        var owner = power.Owner;
        if (!participants.Contains(owner) || owner.IsDead || owner.CombatState != combatState) return;

        if (IntendsToUseSkill(owner))
        {
            onCancel?.Invoke();
            await CreatureCmd.Stun(owner);
        }

        await PowerCmd.Decrement(power);
    }
}

/// <summary>Modelo FGOCore para Servants nuevos. Los modelos publicados pueden delegar al resolver.</summary>
public sealed class SkillSealPower : FGOCorePower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState) =>
        SkillSeal.ResolveTurn(this, choiceContext, participants, combatState, Flash);
}
