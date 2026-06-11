using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>种火/狗粮 — everything is EXP fodder for someone.</summary>
public sealed class ExpEmber() : MemeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var candidates = CardPile.GetCards(Owner, PileType.Hand)
            .Where(c => c != this && !c.IsUpgraded)
            .ToList();
        var rng = Owner.RunState.Rng.CombatCardGeneration;
        for (var i = 0; i < DynamicVars.Cards.IntValue && candidates.Count > 0; i++)
        {
            var pick = candidates[rng.NextInt(candidates.Count)];
            candidates.Remove(pick);
            pick.UpgradeInternal();
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
