using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Constitución Enfermiza A (病弱A, KIT) — DESIGN-OKITA §5.3. 0⚡ Hab, Exhaust: +50★; ganás 1 *Tos
/// (al mazo de robo) (up: +50★ y robá 1). Glow siempre. La skill ES la enfermedad: gloria ahora,
/// tos después.
/// </summary>
public sealed class SicklyConstitution() : OkitaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 50), new CardsVar(0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    // Glow SIEMPRE (siempre paga ★; el downside es moneda).
    protected override bool ShouldGlowGoldInternal => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await Tos.ShuffleIntoDraw(Owner.Creature, this);
        if (DynamicVars.Cards.IntValue > 0) await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m); // robá 0 -> 1
}
