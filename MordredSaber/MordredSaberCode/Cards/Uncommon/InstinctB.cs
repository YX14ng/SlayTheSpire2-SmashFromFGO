using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Instinto B (直感B) — DESIGN-MORDRED §5.2. 1⚡ Hab, Exhaust: +30 Estrellas + robá 1 (up +50★). El
/// S2 base 1:1 (Instinct B: +14★ real, escala-ecosistema ÷10 → +30★). El Exhaust es el cooldown. El
/// robo NO sube con el up (P10). Patrón GraniStalk (★) + robo de RideTheStolen.
/// </summary>
public sealed class InstinctB() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 30), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(20m);
}
