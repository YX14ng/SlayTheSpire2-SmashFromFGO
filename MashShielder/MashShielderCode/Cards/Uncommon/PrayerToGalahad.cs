using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Plegaria a Galahad — double your Block (capped). Exhaust.</summary>
public sealed class PrayerToGalahad() : MashShielderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxBonus", 18)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var gain = Math.Min(Owner.Creature.Block, DynamicVars["MaxBonus"].IntValue);
        if (gain > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, gain, ValueProp.Unpowered, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxBonus"].UpgradeValueBy(7m);
    }
}
