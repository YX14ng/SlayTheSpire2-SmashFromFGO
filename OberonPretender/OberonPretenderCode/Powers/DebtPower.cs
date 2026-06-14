using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Deuda (Debt) -- la identidad de Oberon (DESIGN-OBERON 3.c). Contador (cap 15). Regla:
///
/// Cada carta de PRESTAMO te da recurso AHORA y suma Deuda. Al FINAL de tu turno, tu Carga NP paga
/// tu Deuda (10 NP por punto); la Deuda que no puedas pagar PERSISTE, gana +1 de interes al turno
/// siguiente, y por cada punto impago perdes 3 de Vida (imparable, Unblockable|Unpowered).
///
/// - 1 punto = <see cref="NpPerDebt"/> (10) NP al cobro = <see cref="HpPerUnpaid"/> (3) HP si impago.
/// - Limite de credito (3.c): a Deuda >= <see cref="EffectiveCreditLimit"/> (5; 10 con
///   <see cref="ICreditLimitBooster"/>) no podes jugar Prestamos -- la base OberonCard apaga su glow.
/// - Forma Invierno: el cobro NO ocurre; en su lugar la Deuda gana +1 de interes (bola de nieve).
/// - Forma Vortigern: hasta 5 puntos impagos infligen dano AoE en vez de quitarte Vida
///   (VortigernPower lo maneja en su propio fin de turno; este power cede el cobro de HP ahi).
///
/// El cobro corre en <see cref="BeforeTurnEnd"/> del lado jugador (un solo punto por el que pasa el
/// medidor). Dispara <see cref="IDebtPaidListener"/>/<see cref="IDebtPaidRelicListener"/> con los
/// puntos saldados con NP (para la starter -> Estrellas y Euforia Nocturna).
/// </summary>
public sealed class DebtPower : OberonPower
{
    public const int Max = 15;
    public const int CreditLimit = 5;
    public const int NpPerDebt = 10;
    public const int HpPerUnpaid = 3;
    public const int VortigernUnpaidCap = 5; // hasta 5 puntos impagos viran a AoE en Vortigern

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public static int Of(Creature creature) => creature.GetPowerAmount<DebtPower>();

    /// <summary>Limite de credito vigente (5, o el maximo de los boosters de reliquia).</summary>
    public static int EffectiveCreditLimit(Creature creature)
    {
        var limit = CreditLimit;
        var relics = creature.Player?.Relics;
        if (relics != null)
        {
            foreach (var relic in relics)
            {
                if (relic is ICreditLimitBooster booster) limit = Math.Max(limit, booster.CreditLimit);
            }
        }
        return limit;
    }

    /// <summary>Esta cortado el credito? (los Prestamos dejan de brillar/ser jugables).</summary>
    public static bool CreditCut(Creature creature) => Of(creature) >= EffectiveCreditLimit(creature);

    /// <summary>Apila Deuda respetando el cap (TODA la suma de Deuda pasa por aca).</summary>
    public static async Task Add(Creature creature, int amount, Creature? applier, CardModel? source)
    {
        if (amount <= 0) return;
        var toAdd = Math.Min(amount, Max - Of(creature));
        if (toAdd <= 0) return;
        await PowerCmd.Apply<DebtPower>(creature, toAdd, applier, source);
    }

    /// <summary>Condona (remueve) hasta <paramref name="upTo"/> puntos de Deuda -- Palabras Dulces,
    /// El Final donde Britania..., la Pluma del Contrato. Devuelve lo removido.</summary>
    public static async Task<int> Forgive(Creature creature, int upTo)
    {
        var power = creature.GetPower<DebtPower>();
        if (power == null || upTo <= 0) return 0;
        var removed = Math.Min(upTo, power.Amount);
        if (removed >= power.Amount) await PowerCmd.Remove(power);
        else await PowerCmd.ModifyAmount(power, -removed, creature, null);
        return removed;
    }

    /// <summary>Paga activamente hasta <paramref name="upTo"/> puntos con NP (10 c/u), AHORA, en tu
    /// horario -- Cobro Adelantado. Dispara los listeners de pago. Devuelve lo saldado.</summary>
    public static async Task<int> PayActively(PlayerChoiceContext? choiceContext, Creature creature, int upTo)
    {
        var power = creature.GetPower<DebtPower>();
        if (power == null || upTo <= 0) return 0;
        var paid = await PayWithCharge(creature, Math.Min(upTo, power.Amount));
        if (paid > 0)
        {
            power.Flash();
            await NotifyDebtPaid(choiceContext, creature, paid);
        }
        return paid;
    }

    // Paga `points` con NP (10 c/u, solo lo que el medidor cubre), removiendo esa Deuda. Devuelve lo saldado.
    private static async Task<int> PayWithCharge(Creature creature, int points)
    {
        if (points <= 0) return 0;
        var affordable = Math.Min(points, NpCharge.Current(creature) / NpPerDebt);
        if (affordable <= 0) return 0;
        await NpCharge.Spend(creature, affordable * NpPerDebt, null);
        await Forgive(creature, affordable);
        return affordable;
    }

    private static async Task NotifyDebtPaid(PlayerChoiceContext? choiceContext, Creature creature, int amountPaid)
    {
        foreach (var listener in creature.GetPowerInstances<PowerModel>().OfType<IDebtPaidListener>().ToList())
        {
            await listener.OnDebtPaid(choiceContext, amountPaid);
        }
        var relics = creature.Player?.Relics;
        if (relics != null)
        {
            foreach (var relic in relics)
            {
                if (relic is IDebtPaidRelicListener listener) await listener.OnDebtPaid(choiceContext, amountPaid);
            }
        }
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.IsDead || Amount <= 0) return;

        // Vortigern cede el cobro: su VortigernPower vira la Deuda impaga a dano AoE en su fin de turno.
        if (Owner.HasPower<VortigernPower>()) return;

        // Principe del Invierno: la Deuda NO se cobra; gana +1 de interes (bola de nieve).
        if (Owner.HasPower<WinterPrincePower>())
        {
            Flash();
            await Add(Owner, 1, Owner, null);
            return;
        }

        Flash();

        // 1) Pagar con NP: 10 por punto, hasta lo que el medidor cubra.
        var paid = await PayWithCharge(Owner, Amount);
        if (paid > 0) await NotifyDebtPaid(choiceContext, Owner, paid);

        // 2) Lo impago sangra (3 HP/punto, imparable) -- salvo el perdon del jefe a la 1a impaga.
        var unpaid = Of(Owner);
        if (unpaid > 0)
        {
            var bleeding = unpaid;
            if (ForgivesFirstUnpaid()) bleeding = Math.Max(0, bleeding - 1);
            if (bleeding > 0)
            {
                await CreatureCmd.Damage(choiceContext, Owner, bleeding * HpPerUnpaid,
                    ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            }

            // 3) Interes: cada punto impago gana +1 al turno siguiente (la bola de nieve).
            if (Owner.IsAlive) await Add(Owner, unpaid, Owner, null);
        }
    }

    private bool ForgivesFirstUnpaid()
    {
        var relics = Owner.Player?.Relics;
        if (relics == null) return false;
        foreach (var relic in relics)
        {
            if (relic is IFirstUnpaidDebtForgiver f && f.ForgivesFirstUnpaidDebtHpThisTurn) return true;
        }
        return false;
    }
}
