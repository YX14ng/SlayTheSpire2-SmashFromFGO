using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TiamatBeast.TiamatCode.Powers.Seal;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Sello de Habilidad (スキル封印) — debuff que Tiamat aplica al abrir la ventana Génesis (NP
/// <c>Nammu Dur-an-ki</c>) y vía cartas Lily (Sello de las Mareas). CANCELA la HABILIDAD del enemigo
/// sellado: las acciones NO-ataque (buffs, debuffs, defensa, invocar, curar…) se saltan; los ataques
/// pasan (es Sello de HABILIDAD, no de ataque). Espeja el <c>SleepPower</c> de Oberon, que usa el
/// mismo <see cref="CreatureCmd.Stun"/> para saltar la acción enemiga.
///
/// Arquitectura (los gotchas):
/// - El cancel en el MOMENTO DE SELLAR lo hace el helper <see cref="Sello"/> (Stun si la intención
///   ya roleada es una habilidad). Este power gobierna la DURACIÓN y los turnos SIGUIENTES.
/// - En <see cref="BeforeSideTurnStart"/> del enemigo sellado (antes de que actúe, con el
///   <c>NextMove</c> ya conocido): si intenta una HABILIDAD, la cancela (Stun). Después decrementa el
///   contador 1 (el sello se gasta turno a turno, como la Maldición).
/// - Es lectura/decisión pura sobre la intención: NO toca al jugador ni anula daño (preview-safe).
/// </summary>
public sealed class SkillSealPower : TiamatPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Ofensa personal: no escala en multijugador (espeja CursePower).</summary>
    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>
    /// Antes de que el enemigo sellado actúe (su <c>NextMove</c> ya está roleado): si intenta una
    /// HABILIDAD, la cancelamos (Stun); el ataque pasa. Luego el sello decae 1 (mismo timing/decay
    /// que la Maldición). Va en BeforeSideTurnStart para cancelar ANTES de que la acción se ejecute.
    /// </summary>
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead) return;

        if (Sello.IntendsToUseSkill(Owner))
        {
            Flash();
            await CreatureCmd.Stun(Owner); // la marea ahoga la técnica: la habilidad NO ocurre
        }

        await PowerCmd.Decrement(this);
    }
}
