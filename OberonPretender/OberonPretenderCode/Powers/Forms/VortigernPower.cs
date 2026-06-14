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

    public override string? FramesPath => $"{MainFile.ResPath}/character/oberon_frames_vortigern.tres";

    public bool IgnoresSleep(Creature target) => true; // el abismo devora en el sueno

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 0m;
        return AttackBonus;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        var debt = DebtPower.Of(Owner);
        if (debt <= 0) return;

        Flash();

        // 1) Pagar con NP: 10 por punto, hasta lo que el medidor cubra.
        var affordable = Math.Min(debt, NpCharge.Current(Owner) / DebtPower.NpPerDebt);
        if (affordable > 0)
        {
            await NpCharge.Spend(Owner, affordable * DebtPower.NpPerDebt, null);
            await DebtPower.Forgive(Owner, affordable);
        }

        var unpaid = DebtPower.Of(Owner);
        if (unpaid <= 0) return;

        // 2) Default: hasta 5 puntos impagos viran a dano AoE (2/punto), consumiendo esa Deuda.
        //    Dano directo "el mundo paga la cuenta" (Unblockable|Unpowered): NO es un Ataque, asi que
        //    el +3 de ModifyDamageAdditive (que filtra IsPoweredAttack) no se le suma -- correcto.
        var defaulted = Math.Min(unpaid, DebtPower.VortigernUnpaidCap);
        if (defaulted > 0)
        {
            await DebtPower.Forgive(Owner, defaulted);
            var aoe = defaulted * DamagePerPoint;
            foreach (var enemy in Owner.CombatState.GetOpponentsOf(Owner).ToList())
            {
                if (!enemy.IsDead)
                {
                    await CreatureCmd.Damage(choiceContext, enemy, aoe,
                        ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
                }
            }
        }

        // 3) El excedente sobre el cap si sangra (3 HP/punto, imparable) y gana interes.
        var bleed = DebtPower.Of(Owner);
        if (bleed > 0 && Owner.IsAlive)
        {
            await CreatureCmd.Damage(choiceContext, Owner, bleed * DebtPower.HpPerUnpaid,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            await DebtPower.Add(Owner, bleed, Owner, null);
        }
    }
}
