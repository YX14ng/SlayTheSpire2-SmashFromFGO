using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Promesa bajo la Tormenta — Habilidad 2⚡, Exhaust: ganás 2 Anti-Purga.
/// Mejora: 2 Anti-Purga y 2★. Co-op: cada aliado gana 1 Anti-Purga (las ★ quedan self).
/// </summary>
public sealed class PromiseUnderTheStorm() : ArtoriaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AntiPurge", 2),
        new DynamicVar("Stars", 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<AntiPurgePower>(), HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AntiPurgePower>(Owner.Creature, DynamicVars["AntiPurge"].BaseValue, Owner.Creature, this);
        if (DynamicVars["Stars"].IntValue > 0)
        {
            await Stars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        }

        // Co-op: la promesa cubre a todo el party con 1 Anti-Purga (las ★ son self, no compartibles).
        await ForEachAlly(async ally =>
            await PowerCmd.Apply<AntiPurgePower>(ally, 1m, Owner.Creature, this));
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(2m);
    }
}
