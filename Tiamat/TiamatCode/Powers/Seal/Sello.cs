using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using FGOCore.FGOCoreCode.Seal;

namespace TiamatBeast.TiamatCode.Powers.Seal;

/// <summary>
/// Helper de la economía del Sello de Habilidad (スキル封印) — espejo del <c>Sleep</c> de Oberon
/// (OberonPretenderCode/Powers/Sleep/Sleep.cs) y de <c>Curses</c>/<c>Lahmu</c>. TODA aplicación de
/// Sello pasa por acá para enganchar el "cancelar la habilidad enemiga" de forma uniforme.
///
/// QUÉ HACE (de verdad, no placeholder): un Sello válido CANCELA la habilidad (la acción NO-ataque)
/// del enemigo. El motor del juego rola la intención del enemigo al INICIO de TU turno
/// (<c>CombatManager.PrepareForNextTurn</c> → <c>RollMove</c>), así que cuando jugás una carta que
/// sella, la intención ya es visible. Si esa intención es una HABILIDAD (no <c>IntendsToAttack</c>),
/// la cancelamos con <see cref="CreatureCmd.Stun"/> (mismo mecanismo que el Sueño de Oberon: la
/// próxima acción se reemplaza por STUNNED, saltándola; FollowUpStateId re-encola el move original).
/// Si la intención es un ATAQUE, el Sello NO lo cancela (es Sello de HABILIDAD, no de ataque): el
/// power persiste y <see cref="SkillSealPower"/> cancelará la próxima habilidad que el enemigo
/// intente mientras dure el sello.
/// </summary>
public static class Sello
{
    public const int DefaultDuration = 1;

    public static bool IsSealed(Creature creature) => creature.HasPower<SkillSealPower>();

    /// <summary>¿La intención ACTUAL del enemigo es una habilidad sellable (no un ataque)?
    /// Lectura pura sobre el <c>NextMove</c> ya roleado. Un STUNNED (ya cancelado) no es sellable.</summary>
    public static bool IntendsToUseSkill(Creature creature) => SkillSeal.IntendsToUseSkill(creature);

    /// <summary>
    /// Sella al objetivo: aplica el <see cref="SkillSealPower"/> (duración en Counter) y, si su
    /// intención ACTUAL es una habilidad, la CANCELA ya mismo (Stun). Devuelve true si selló.
    /// </summary>
    public static Task<bool> Apply(
        Creature target,
        int duration,
        Creature? applier,
        MegaCrit.Sts2.Core.Models.CardModel? source) =>
        Apply(new BlockingPlayerChoiceContext(), target, duration, applier, source);

    public static Task<bool> Apply(
        PlayerChoiceContext choiceContext,
        Creature target,
        int duration,
        Creature? applier,
        MegaCrit.Sts2.Core.Models.CardModel? source)
        => SkillSeal.Apply<SkillSealPower>(choiceContext, target, duration, applier, source);
}
