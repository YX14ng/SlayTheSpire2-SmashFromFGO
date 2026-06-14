using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Postura Veloz (缩地·攻势) — ESTE TURNO tus Ataques hacen +<see cref="Amount"/> de daño.
/// El estado pre-ataque ATK+20% del Shukuchi B+ real (DESIGN-OKITA §2). Aditivo a TODOS tus
/// Ataques (no solo críticos). Se auto-remueve al terminar tu turno. Counter: suma.
/// </summary>
public sealed class SwiftStancePower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        return Amount;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}
