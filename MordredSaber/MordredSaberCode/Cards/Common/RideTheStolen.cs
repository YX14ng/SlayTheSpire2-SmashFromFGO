using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Cabalgar lo Robado (骑乘掠夺) — DESIGN-MORDRED §5.1. 1⚡ Hab: robá 2 + 10 Estrellas (up: robá 3).
/// El Riding B (sabe montar de todo, hasta lo robado): ciclo de cartas con feed de ★. Patrón
/// BattleInstinct (CardPileCmd.Draw con CardsVar) + GraniStalk (CritStars.Gain). El up sube SOLO el
/// robo; el ★ queda fijo (no apilar dos perillas en el up).
/// </summary>
public sealed class RideTheStolen() : MordredCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
