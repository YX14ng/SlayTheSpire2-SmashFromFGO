using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Saturación de Eventos (事象饱和) — ESTE TURNO tus Ataques IGNORAN Bloqueo (DESIGN-OKITA §5.4:
/// la paradoja como skill, anti torre-de-bloqueo). La aplica «Saturación de Eventos» (2⚡, Hab,
/// Exhaust). Se auto-remueve al terminar tu turno (patrón GloryEdgePower / ExposedBackPower).
///
/// Equivalente funcional de Unblockable genérico con APIs verificadas (DESIGN-OKITA §4): antes de
/// cada Ataque de carta del owner (hook <see cref="BeforeAttack"/>), vacía el Bloqueo de todos los
/// enemigos (CreatureCmd.LoseBlock + GetOpponentsOf). Single, personal: no escala en multijugador.
/// </summary>
public sealed class EventSaturationPower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task BeforeAttack(AttackCommand command)
    {
        await base.BeforeAttack(command);
        if (command.Attacker != Owner || command.ModelSource is not CardModel || Owner.CombatState == null) return;
        Flash();
        foreach (var enemy in Owner.CombatState.GetOpponentsOf(Owner).ToList())
        {
            if (enemy.IsDead || enemy.Block <= 0) continue;
            await CreatureCmd.LoseBlock(enemy, enemy.Block);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}
