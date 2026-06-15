using MegaCrit.Sts2.Core.Entities.Relics;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// El Libro del Fin de los Sueños (梦终之书 / The Book of Dream's End) — reliquia de JEFE/ANCIENT
/// (DESIGN-OBERON §7 #4): reemplaza a la starter de motor por un banco mas generoso y misericordioso.
/// (1) Cada punto de Deuda que PAGAS con NP al cobro: +10 Estrellas (max 5/turno) — sube el cap de la
///     starter de 3 a 5 (el motor de Estrellas vive en <see cref="DebtPaidStarsRelic"/>).
/// (2) La PRIMERA Deuda impaga de cada turno NO te quita Vida (solo gana interes) —
///     <see cref="IFirstUnpaidDebtForgiver"/>.
/// (3) El limite de credito sube de 5 a 10 (<see cref="ICreditLimitBooster"/>).
/// </summary>
public sealed class BookOfDreamsEnd : DebtPaidStarsRelic, IFirstUnpaidDebtForgiver, ICreditLimitBooster
{
    public const int BoostedCreditLimit = 10;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public bool ForgivesFirstUnpaidDebtHpThisTurn => true;

    public int CreditLimit => BoostedCreditLimit;

    protected override int StarsPerDebtPaid => 10;

    protected override int MaxProcsPerTurn => 5;
}
