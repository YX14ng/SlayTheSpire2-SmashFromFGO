using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>Resolución Valiente — doble-feeder defensa+motor (NO da SdD, evita el doble-dip de
/// mitigación). SKILL §2: bloqueo 1⚡ con rider = 4-7; 6 + rider 10 NP. El up sube SOLO el NP (+10).</summary>
public sealed class BraveResolve() : SiegfriedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(10m);
}
