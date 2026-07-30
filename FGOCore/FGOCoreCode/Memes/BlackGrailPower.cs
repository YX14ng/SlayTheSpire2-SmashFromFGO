using System.Collections.Generic;
using FGOCore.FGOCoreCode.Compatibility;
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

    // Counter (2026-07-04, reporte de player: "jugar el segundo Grial no hace nada"): cada copia suma
    // una acumulación = +50% de daño y +4 HP/turno MÁS. Misma tasa por stack que la carta promete;
    // antes era Single y la segunda copia se perdía en silencio.
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageMultiplicativeFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 1m;
        // +50% por acumulación, aditivo (1 stack ×1.5, 2 stacks ×2.0, ...).
        return 1m + (DamageMultiplier - 1m) * Math.Max(1m, Amount);
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.IsDead) return;
        Flash();
        await CreatureCmdCompatibility.Damage(new ThrowingPlayerChoiceContext(), Owner,
            HpLossPerTurn * (int)Math.Max(1m, Amount),
            ValueProp.Unblockable | ValueProp.Unpowered, null, null, null);
    }
}
