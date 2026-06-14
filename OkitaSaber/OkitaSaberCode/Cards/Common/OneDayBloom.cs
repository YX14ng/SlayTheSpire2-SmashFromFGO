using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Flor de un Día (一日之花) — DESIGN-OKITA §5.2. 1⚡ Hab, Exhaust: +30★; robá 1 (up: +50★). Gloria
/// efímera (el Exhaust ES lo efímero).
/// </summary>
public sealed class OneDayBloom() : OkitaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 30), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(20m); // +30 -> +50
}
