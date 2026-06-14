using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Cards;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Paso Constante (稳步) — la PRIMERA *Ráfaga de cada turno reembolsa 1 *Aliento (up: y +5★)
/// (DESIGN-OKITA §5.3). Descuento de Aliento capeado a 1/turno. <see cref="Rafaga.Pay"/> consulta
/// <see cref="IFirstRafagaRefund"/> al pagar una Ráfaga; <see cref="TryConsumeRefund"/> devuelve
/// true una sola vez por turno. Single.
/// </summary>
public sealed class SteadyStepPower : OkitaPower, IFirstRafagaRefund
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>★ extra del reembolso (0 base; 5 con la mejora — la carta lo setea).</summary>
    public int RefundStarsValue = 0;

    private bool _usedThisTurn;

    public int RefundAmount => 1;

    public int RefundStars => RefundStarsValue;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    public bool TryConsumeRefund()
    {
        if (_usedThisTurn) return false;
        _usedThisTurn = true;
        Flash();
        return true;
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) _usedThisTurn = false;
        return Task.CompletedTask;
    }
}
