using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Sándwich de Chaldea — heal behind a solid wall. Exhaust.</summary>
public sealed class ChaldeaSandwich() : MashShielderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int RequiredBlock = 12;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(5m),
        new DynamicVar("RequiredBlock", RequiredBlock)
    ];

    protected override bool IsPlayable => Owner.Creature.Block >= RequiredBlock;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(3m);
    }
}
