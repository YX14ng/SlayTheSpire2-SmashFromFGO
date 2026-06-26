using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Égida Feérica — ganás 1 Anti-Purga; robás 1. Mejorada: robás 2. Rescate P2 2026-06-25: se le
/// sacó el *Exhaust y el cantrip (robás 1) pasó a ser base — era un setup Exhaust dominado; ahora
/// es un escudo anti-purga reciclable que no descarta cartas de tu mazo de un solo uso.
/// </summary>
public sealed class FaerieAegis() : ArtoriaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AntiPurge", 1),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AntiPurgePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AntiPurgePower>(choiceContext, Owner.Creature, DynamicVars["AntiPurge"].BaseValue, Owner.Creature, this);
        if (DynamicVars.Cards.IntValue > 0)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
