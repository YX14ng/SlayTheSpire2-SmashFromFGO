using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>Mantenimiento del Ortinax — convert all Block into NP Charge (capped).</summary>
public sealed class OrtinaxMaintenance() : MashShielderCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxCharge", 30)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = Owner.Creature.Block;
        if (block <= 0) return;

        await CreatureCmd.LoseBlock(Owner.Creature, block);
        await NpCharge.Gain(Owner.Creature, Math.Min(block, DynamicVars["MaxCharge"].IntValue), this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxCharge"].UpgradeValueBy(10m);
    }
}
