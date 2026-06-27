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
        // IsUpgradable (no !IsUpgraded): excluye cartas con MaxUpgradeLevel==0 (no-mejorables) que
        // pasarian el filtro de !IsUpgraded y se romperian al empujarlas por encima del tope.
        var candidates = CardPile.GetCards(Owner, PileType.Hand)
            .Where(c => c != this && c.IsUpgradable)
            .ToList();
        var rng = Owner.RunState.Rng.CombatCardGeneration;
        for (var i = 0; i < DynamicVars.Cards.IntValue && candidates.Count > 0; i++)
        {
            var pick = candidates[rng.NextInt(candidates.Count)];
            candidates.Remove(pick);
            // CardCmd.Upgrade (no UpgradeInternal crudo): corre Internal+Finalize (limpia el highlight
            // verde "recien mejorada") + dispara el preview visual, igual que vanilla Armaments.
            CardCmd.Upgrade(pick);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
