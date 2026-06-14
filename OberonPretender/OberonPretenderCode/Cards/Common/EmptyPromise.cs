using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Stars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Promesa Vacía (空头承诺 / Empty Promise) — DESIGN-OBERON §6.2. 0⚡ Habilidad · PRÉSTAMO:
/// +50 Estrellas (up +60). Deuda 2. El préstamo de estrellas, cobrado en NP (interés cruzado): el
/// recurso que pedís es ★ pero la cuenta la paga el medidor de NP — la trampa del prestamista que da
/// una cosa y cobra otra. <see cref="ILoanCard"/> → endulzante del Rey + «Interés»; crédito 5.
/// </summary>
public sealed class EmptyPromise() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.Self), ILoanCard
{
    private const int Stars = 50;
    public const int Debt = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Stars", Stars),
        new DynamicVar("Debt", Debt)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
