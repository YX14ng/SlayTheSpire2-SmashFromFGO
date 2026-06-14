using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Negociación Feérica (妖精交涉 / Faerie Negotiation) — DESIGN-OBERON §6.3. 0⚡ Habilidad: +10 Estrellas
/// por cada punto de Deuda que tengas (máx 3); tu Deuda NO se reduce (up máx 5). La Deuda como ACTIVO
/// (sinergia Invierno). Glow si tenés Deuda.
/// </summary>
public sealed class FaerieNegotiation() : OberonCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int StarsPerDebt = 10;
    private const int MaxDebt = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxDebt", MaxDebt)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DebtPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var counted = Math.Min(DebtPower.Of(Owner.Creature), DynamicVars["MaxDebt"].IntValue);
        if (counted > 0)
        {
            await CritStars.Gain(Owner.Creature, counted * StarsPerDebt, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["MaxDebt"].UpgradeValueBy(2m);
}
