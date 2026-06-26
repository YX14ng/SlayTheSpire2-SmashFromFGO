using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// El Santo Grial Negro (黑圣杯) — glass-cannon: tus Ataques con poder hacen +50%
/// (×1.5 multiplicativo) pero al inicio de cada uno de tus turnos perdés HP igual a
/// <see cref="HpLossPerTurn"/>. Power permanente (dura todo el combate). Mismo gate que
/// <c>DoubleDamagePower</c> vanilla (<c>IsPoweredAttack</c> + dealer = dueño); espejo del
/// daño-por-turno de <c>CursePower.AfterSideTurnStart</c> (Unblockable | Unpowered).
/// Personal: no escala en multijugador.
/// </summary>
public sealed class BlackGrailPower : FGOCorePower
{
    /// <summary>HP que el dueño paga al inicio de cada uno de sus turnos.</summary>
    public const int HpLossPerTurn = 4;

    /// <summary>Multiplicador de daño de los Ataques con poder (×1.5 = +50%).</summary>
    public const decimal DamageMultiplier = 1.5m;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 1m;
        return DamageMultiplier;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, HpLossPerTurn,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }
}
