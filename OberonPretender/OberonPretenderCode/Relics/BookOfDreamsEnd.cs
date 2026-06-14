using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// El Libro del Fin de los Sueños (梦终之书 / The Book of Dream's End) — reliquia de JEFE/ANCIENT
/// (DESIGN-OBERON §7 #4): reemplaza a la starter de motor por un banco mas generoso y misericordioso.
/// (1) Cada punto de Deuda que PAGAS con NP al cobro: +10 Estrellas (max 5/turno) — sube el cap de la
///     starter de 3 a 5 (<see cref="IDebtPaidRelicListener"/>).
/// (2) La PRIMERA Deuda impaga de cada turno NO te quita Vida (solo gana interes) —
///     <see cref="IFirstUnpaidDebtForgiver"/>.
/// (3) El limite de credito sube de 5 a 10 (<see cref="ICreditLimitBooster"/>).
/// </summary>
public sealed class BookOfDreamsEnd : OberonRelic, IDebtPaidRelicListener, IFirstUnpaidDebtForgiver, ICreditLimitBooster
{
    public const int StarsPerDebtPaid = 10;
    public const int MaxProcsPerTurn = 5;
    public const int BoostedCreditLimit = 10;

    private int _procsThisTurn;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public bool ForgivesFirstUnpaidDebtHpThisTurn => true;

    public int CreditLimit => BoostedCreditLimit;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DebtPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _procsThisTurn = 0;
        return Task.CompletedTask;
    }

    public async Task OnDebtPaid(PlayerChoiceContext? choiceContext, int amountPaid)
    {
        var procs = Math.Min(amountPaid, MaxProcsPerTurn - _procsThisTurn);
        if (procs <= 0) return;
        _procsThisTurn += procs;
        Flash();
        await CritStars.Gain(Owner.Creature, procs * StarsPerDebtPaid, null);
    }
}
