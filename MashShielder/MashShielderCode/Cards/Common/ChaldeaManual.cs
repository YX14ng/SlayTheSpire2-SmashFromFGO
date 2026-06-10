using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>Manual de Chaldea — draw 3, discard 1 (upgraded: no discard).</summary>
public sealed class ChaldeaManual() : MashShielderCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new DynamicVar("Discard", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        var toDiscard = DynamicVars["Discard"].IntValue;
        if (toDiscard <= 0) return;

        var selected = await CardSelectCmd.FromHandForDiscard(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, toDiscard, toDiscard), null, this);
        await CardCmd.DiscardAndDraw(choiceContext, selected, 0);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Discard"].UpgradeValueBy(-1m);
    }
}
