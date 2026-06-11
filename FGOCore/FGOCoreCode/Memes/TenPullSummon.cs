using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>十连 — roll the gacha: whatever comes, comes (歪了 included).</summary>
public sealed class TenPullSummon() : MemeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pulls = CardFactory.GetDistinctForCombat(Owner,
            Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, RunState.CardMultiplayerConstraint),
            DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration);
        foreach (var card in pulls)
        {
            card.EnergyCost.SetThisCombat(0);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 1.0f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
