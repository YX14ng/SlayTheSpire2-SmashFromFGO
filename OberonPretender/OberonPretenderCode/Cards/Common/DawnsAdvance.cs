using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Anticipo del Alba (黎明预支 / Dawn's Advance) — DESIGN-OBERON §6.2. 0⚡ Habilidad · PRÉSTAMO:
/// +50 Carga NP (up +60). Deuda 2. El «Moli1 con interés»: ráfaga +50 ya, neto +30 si ahorrás (el
/// cobro toma 20 al amanecer). El préstamo estándar de batería a 0⚡: cruzás los 100 antes que nadie.
/// <see cref="ILoanCard"/> → endulzante del Rey + «Interés a Mi Favor»; crédito cortado a Deuda ≥ 5.
/// </summary>
public sealed class DawnsAdvance() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.Self), ILoanCard
{
    private const int Charge = 50;
    public const int Debt = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Charge", Charge),
        new DynamicVar("Debt", Debt)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Charge"].UpgradeValueBy(10m);
}
