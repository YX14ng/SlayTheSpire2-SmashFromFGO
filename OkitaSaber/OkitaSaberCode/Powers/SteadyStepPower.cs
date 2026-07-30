using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
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
    public int RefundStarsValue => FgoCombatState.GetCombat(Owner, 5, 3);

    public int RefundAmount => 1;

    public int RefundStars => RefundStarsValue;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    public Task Configure(PlayerChoiceContext context, int refundStars, CardModel source) =>
        FgoCombatState.SetCombat(
            context, Owner, 5, Math.Max(RefundStarsValue, refundStars), source, width: 3);

    public async Task<bool> TryConsumeRefund(PlayerChoiceContext context, CardModel? source)
    {
        if (FgoCombatState.GetTurn(Owner, 2) != 0) return false;
        await FgoCombatState.SetTurn(context, Owner, 2, 1, source);
        Flash();
        return true;
    }
}
