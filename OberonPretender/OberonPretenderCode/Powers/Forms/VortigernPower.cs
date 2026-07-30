using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers.Sleep;

namespace OberonPretender.OberonPretenderCode.Powers.Forms;

/// <summary>
/// VORTIGERN -- la forma CLIMAX PERMANENTE (modelo 2800120, alas venosas de libelula).
/// <see cref="IsPermanent"/>: una vez insecto, no se vuelve atras. Pasiva invertida (DESIGN-OBERON 5):
///
/// Tus Ataques hacen +<see cref="AttackBonus"/> y golpean a los Dormidos SIN despertarlos
///  (<see cref="ISleepIgnorer"/>). Al final de tu turno, hasta <see cref="DebtPower.VortigernUnpaidCap"/>
///  (5) puntos de Deuda impaga infligen <see cref="DamagePerPoint"/> (2) de dano a TODOS por punto en
///  vez de quitarte Vida; el resto se cobra normal (NP primero). Pierde el endulzante del Rey.
///
/// Vortigern OWN-ea TODO el cobro de fin de turno (DebtPower cede cuando esta forma esta presente):
/// 1) paga con NP (10/punto), 2) hasta 5 puntos impagos restantes -> declaran default (2 AoE/punto), el
/// excedente si sangra. La decision se invierte: ahora queres estar en cero, fundir el medidor cada
/// turno y que el mundo pague la cuenta.
/// </summary>
public sealed class VortigernPower : OberonFormPower, ISleepIgnorer
{
    public const int AttackBonus = 3;
    public const int DamagePerPoint = 2;

    public override bool IsPermanent => true;

    public override string FramesPath => $"{MainFile.ResPath}/character/oberon_frames_vortigern.tres";

    public bool IgnoresSleep(Creature target) => true; // el abismo devora en el sueno

    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 0m;
        return AttackBonus;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.IsDead) return;
        var debt = DebtPower.Of(Owner);
        if (debt <= 0) return;

        Flash();

        // 1) Pagar con NP por el camino UNICO de DebtPower (audit 2026-07-04): el cobro inline no
        //    disparaba NotifyDebtPaid — Dream Contract y el Libro del Fin de los Suenos dejaban de
        //    dar Estrellas en Vortigern. PayActively paga (10 NP/punto) y notifica con el contexto.
        await DebtPower.PayActively(choiceContext, Owner, debt);

        var unpaid = DebtPower.Of(Owner);
        if (unpaid <= 0) return;

        // 2) Default: hasta 5 puntos impagos viran a dano AoE (2/punto), consumiendo esa Deuda.
        //    Dano directo "el mundo paga la cuenta" (Unblockable|Unpowered): NO es un Ataque, asi que
        //    el +3 de ModifyDamageAdditive (que filtra IsPoweredAttack) no se le suma -- correcto.
        var defaulted = Math.Min(unpaid, DebtPower.VortigernUnpaidCap);
        if (defaulted > 0)
        {
            await DebtPower.Forgive(choiceContext, Owner, defaulted);
            var aoe = defaulted * DamagePerPoint;
            if (Owner.CombatState is not { } combatState) return;
            foreach (var enemy in combatState.GetOpponentsOf(Owner).ToList())
            {
                if (!enemy.IsDead)
                {
                    await CreatureCmd.Damage(choiceContext, enemy, aoe,
                        ValueProp.Unblockable | ValueProp.Unpowered, Owner);
                }
            }
        }

        // 3) El excedente sobre el cap si sangra (3 HP/punto, imparable) y gana interes.
        var bleed = DebtPower.Of(Owner);
        if (bleed > 0 && Owner.IsAlive)
        {
            await CreatureCmdCompatibility.Damage(choiceContext, Owner, bleed * DebtPower.HpPerUnpaid,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null, null);
            await DebtPower.Add(choiceContext, Owner, 1, Owner, null);
        }
    }
}
