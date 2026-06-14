using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Hipoteca del Mañana (抵押明天 / Mortgage Tomorrow) — DESIGN-OBERON §6.3. 2⚡ Habilidad · Exhaust ·
/// PRÉSTAMO: +100 Carga NP. Deuda 4 (up Deuda 3). El jackpot: ulti instantáneo, neto +60 pagado con
/// 2⚡ + Exhaust + Deuda 4. <see cref="ILoanCard"/> → endulzante del Rey + «Interés a Mi Favor»;
/// crédito cortado a Deuda ≥ 5.
/// </summary>
public sealed class MortgageTomorrow() : OberonCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self), ILoanCard
{
    public const int Debt = 4;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Charge", 100), new DynamicVar("Debt", Debt)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Debt"].UpgradeValueBy(-1m);
}
