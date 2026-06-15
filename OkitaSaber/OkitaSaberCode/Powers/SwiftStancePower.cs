using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Postura Veloz (缩地·攻势) — ESTE TURNO tus Ataques hacen +<see cref="Amount"/> de daño.
/// El estado pre-ataque ATK+20% del Shukuchi B+ real (DESIGN-OKITA §2). Aditivo a TODOS tus
/// Ataques (no solo críticos). Se auto-remueve al terminar tu turno. Counter: suma.
/// </summary>
public sealed class SwiftStancePower : AttackDamageAdditivePower
{
    // Aplica a TODOS tus Ataques (no requiere Crítico Listo): hereda BonusApplies() => true.

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}
