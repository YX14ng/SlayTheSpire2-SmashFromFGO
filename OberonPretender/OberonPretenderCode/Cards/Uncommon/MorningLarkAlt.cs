using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Alondra de la Mañana+ (朝之云雀 / Morning Lark EX alt) — DESIGN-OBERON §6.3, variante draft del S2.
/// 1⚡ Habilidad · Exhaust · PRÉSTAMO: +50 Carga NP; +20 Estrellas. Deuda 2 (up +10 NP / +10 Estrellas).
/// El préstamo doble (NP + estrellas) de un solo uso. <see cref="ILoanCard"/> → endulzante del Rey +
/// «Interés a Mi Favor»; crédito cortado a Deuda ≥ 5.
/// </summary>
public sealed class MorningLarkAlt() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), ILoanCard
{
    public const int Debt = 2;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Charge", 50),
        new DynamicVar("Stars", 20),
        new DynamicVar("Debt", Debt)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Charge"].UpgradeValueBy(10m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
