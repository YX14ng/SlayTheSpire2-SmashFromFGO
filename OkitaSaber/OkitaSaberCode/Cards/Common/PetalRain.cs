using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Lluvia de Pétalos (花吹雪) — DESIGN-OKITA §5.2. 1⚡ Hab: +20★; robá 1 (up: +30★). Paquete ★.
/// </summary>
public sealed class PetalRain() : OkitaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 20), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m); // +20 -> +30
}
