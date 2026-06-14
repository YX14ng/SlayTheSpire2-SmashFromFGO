using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Lluvia de Estrellas Falsas (伪星之雨 / False Starfall) — DESIGN-OBERON §6.3. 1⚡ Habilidad · PRÉSTAMO:
/// +50 Estrellas; robá 1. Deuda 2 (up robá 2). El préstamo de estrellas, cobrado en NP (interés cruzado).
/// <see cref="ILoanCard"/> → endulzante del Rey + «Interés a Mi Favor»; crédito cortado a Deuda ≥ 5.
/// </summary>
public sealed class FalseStarfall() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), ILoanCard
{
    public const int Debt = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Stars", 50),
        new DynamicVar("Draw", 1),
        new DynamicVar("Debt", Debt)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}
