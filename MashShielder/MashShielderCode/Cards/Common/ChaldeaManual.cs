using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Manual de Chaldea — retoque v2: 1E Habilidad, roba 3, descarta 1, +10 Estrellas
/// de Crítico (up: +20 estrellas; conserva el descarte — parche MENORES del juez:
/// la predicción mejora, la mano no se infla). Teoría de combate → predicción.
/// </summary>
public sealed class ChaldeaManual() : MashShielderCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new DynamicVar("Discard", 1),
        new DynamicVar("Stars", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        var toDiscard = DynamicVars["Discard"].IntValue;
        if (toDiscard > 0)
        {
            var selected = await CardSelectCmd.FromHandForDiscard(choiceContext, Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, toDiscard, toDiscard), null, this);
            await CardCmd.DiscardAndDraw(choiceContext, selected, 0);
        }

        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
