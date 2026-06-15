using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Base local de las reliquias-MOTOR de Oberon que convierten el pago de Deuda en Estrellas
/// (El Contrato de Sueños starter y El Libro del Fin de los Sueños jefe). Las dos compartían
/// byte-a-byte el listener <see cref="IDebtPaidRelicListener"/>: +<see cref="StarsPerDebtPaid"/>
/// Estrellas por punto de Deuda saldado con NP, capado a <see cref="MaxProcsPerTurn"/> procs/turno
/// (reset en <see cref="AfterSideTurnStart"/> del lado jugador). Sólo el cap y la cantidad por proc
/// varían entre ellas; las subclases declaran esas constantes y su rareza/efectos extra.
///
/// Comportamiento idéntico al que tenían inline: misma fórmula, mismo Flash, mismo HoverTip de
/// Deuda + Estrellas de Crítico.
/// </summary>
public abstract class DebtPaidStarsRelic : OberonRelic, IDebtPaidRelicListener
{
    /// <summary>Estrellas concedidas por cada punto de Deuda saldado con NP.</summary>
    protected abstract int StarsPerDebtPaid { get; }

    /// <summary>Tope de procs (puntos premiados) por turno.</summary>
    protected abstract int MaxProcsPerTurn { get; }

    private int _procsThisTurn;

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
