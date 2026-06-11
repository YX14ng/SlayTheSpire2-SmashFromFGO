using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Formación Defensiva — this turn, Block cards are played twice. Exhaust (upgrade removes it).</summary>
public sealed class DefensiveFormation() : MashShielderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [] : [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DefensiveFormationPower>("DefensiveFormation", 99m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DefensiveFormationPower>(Owner.Creature, DynamicVars["DefensiveFormation"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}
