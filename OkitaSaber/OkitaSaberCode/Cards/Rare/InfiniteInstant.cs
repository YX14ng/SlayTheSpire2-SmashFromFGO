using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Instante Infinito (无限之刹) — DESIGN-OKITA §5.4. 2⚡ Hab, Exhaust: +1 Intangible; +30★ (up: además
/// robá 2). Botón de pánico defensivo.
/// </summary>
public sealed class InfiniteInstant() : OkitaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<IntangiblePower>("Intangible", 1m),
        new DynamicVar("Stars", 30),
        new CardsVar(0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<IntangiblePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IntangiblePower>(Owner.Creature, DynamicVars["Intangible"].BaseValue, Owner.Creature, this);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        if (DynamicVars.Cards.IntValue > 0) await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(2m); // robá 0 -> 2
}
