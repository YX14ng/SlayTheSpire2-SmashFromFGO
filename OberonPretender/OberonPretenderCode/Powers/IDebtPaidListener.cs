using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Un power que reacciona cuando su dueno PAGA Deuda con Carga NP al cobro de fin de turno
/// (el evento <c>AfterDebtPaid</c> del diseno 4). Lo usa Euforia Nocturna y cualquier motor
/// que premie la solvencia. La starter El Contrato de Suenos y el jefe Libro del Fin de los
/// Suenos leen el MISMO evento desde el lado reliquia (<see cref="IDebtPaidRelicListener"/>).
///
/// <paramref name="amountPaid"/> = puntos de Deuda saldados con NP este cobro (ya capado por lo
/// que el medidor pudo cubrir; NO incluye lo condonado por cartas ni lo impago).
/// </summary>
public interface IDebtPaidListener
{
    Task OnDebtPaid(PlayerChoiceContext? choiceContext, int amountPaid);
}

/// <summary>Version lado-reliquia del listener de pago de Deuda (la starter / el jefe).</summary>
public interface IDebtPaidRelicListener
{
    Task OnDebtPaid(PlayerChoiceContext? choiceContext, int amountPaid);
}

/// <summary>
/// Una reliquia que evita que la PRIMERA Deuda impaga de cada turno te quite Vida (solo gana
/// interes): el jefe El Libro del Fin de los Suenos (7 #4). <see cref="DebtPower"/> lo consulta
/// como lectura pura en el cobro.
/// </summary>
public interface IFirstUnpaidDebtForgiver
{
    bool ForgivesFirstUnpaidDebtHpThisTurn { get; }
}

/// <summary>
/// Una reliquia que sube el limite de credito de Oberon (de 5 a 10): El Libro del Fin de los
/// Suenos y el Trono del Invierno. <see cref="DebtPower"/> toma el MAXIMO de los boosters.
/// </summary>
public interface ICreditLimitBooster
{
    int CreditLimit { get; }
}
