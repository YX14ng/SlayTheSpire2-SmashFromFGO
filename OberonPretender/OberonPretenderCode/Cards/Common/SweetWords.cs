using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Palabras Dulces (甜言蜜语 / Sweet Words) — DESIGN-OBERON §6.2. 0⚡ Habilidad · Exhaust: remové
/// <see cref="Forgiven"/> punto(s) de tu Deuda (up 2). La condonación pura (sin pago de NP ni HP): el
/// Exhaust paga el perdón (§3). A diferencia del Cobro Adelantado, NO dispara las Estrellas de la
/// starter (no es un pago, es un borrado). Glow si tenés Deuda.
/// </summary>
public sealed class SweetWords() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int Forgiven = 1;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Debt", Forgiven)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DebtPower.Forgive(Owner.Creature, DynamicVars["Debt"].IntValue);
    }

    protected override void OnUpgrade() => DynamicVars["Debt"].UpgradeValueBy(1m);
}
