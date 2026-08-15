using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Regalo de Vivian (薇薇安的赠礼) — añade 2 Armas del Caballero a tu mano.
/// Exhaust. Mejora: añade 3 (simplificación del "+3 de daño a tus Armas").
/// </summary>
public sealed class ViviansGift() : MorganCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Special.KnightsArm.AddToHand(Owner.Creature, DynamicVars.Cards.IntValue, IsUpgraded);
    }

    // RE-POOL V2: la mejora no agrega más Arms — las trae MEJORADAS (burst, no motor; J1-11/J3-5).
    protected override void OnUpgrade()
    {
    }
}
