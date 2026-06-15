using FGOCore.FGOCoreCode.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Reclamo de Caliburn (夺取石中剑) — DESIGN-MORDRED §5.3. 2⚡ Hab, Exhaust: agregá a tu mano 1 carta
/// RARA aleatoria de tu pool; cuesta 0 este turno (up: 1⚡). Su deseo: sacar la espada de la selección
/// — el pool se cita a sí mismo (slot Magia de Proyección). Patrón TenPullSummon de FGOCore filtrado a
/// rareza Rara: CardFactory.GetDistinctForCombat sobre el pool desbloqueado + SetThisCombat(0).
/// </summary>
public sealed class ClaimCaliburn() : MordredCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pool = Owner.Character?.CardPool;
        var runState = Owner.RunState;
        if (pool == null || runState == null) return;

        var rares = pool
            .GetUnlockedCards(Owner.UnlockState, runState.CardMultiplayerConstraint)
            .Where(c => c.Rarity == CardRarity.Rare && c.GetType() != GetType())
            .ToList();

        var picks = CardFactory.GetDistinctForCombat(Owner, rares,
            DynamicVars.Cards.IntValue, runState.Rng.CombatCardGeneration);
        foreach (var card in picks)
        {
            card.EnergyCost.SetThisCombat(0);
            await ManifestCards.ManifestToHand(Owner.Creature, card);
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
