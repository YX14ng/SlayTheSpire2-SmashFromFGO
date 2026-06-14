using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Cobro Adelantado (提前催收 / Early Collection) — DESIGN-OBERON §6.2. 1⚡ Habilidad: pagá YA hasta
/// <see cref="MaxPaid"/> puntos de tu Deuda (con NP, 10 c/u); robá 1 (up robá 2). Pagar en TU horario
/// dispara las Estrellas de la starter cuando querés y desactiva el cobro de fin de turno — el control
/// activo de la Deuda. Reusa <see cref="DebtPower.PayActively"/> (que dispara los IDebtPaidListener,
/// compartiendo el cap de 3 procs/turno con el cobro automático). Glow si tenés Deuda.
/// </summary>
public sealed class EarlyCollection() : OberonCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int MaxPaid = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Pay", MaxPaid),
        new DynamicVar("Draw", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DebtPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DebtPower.PayActively(choiceContext, Owner.Creature, DynamicVars["Pay"].IntValue);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}
