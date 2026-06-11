using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>#18 Canto de la Lluvia (雨之咏唱) — 5 de Bloqueo; en forma Bruja: Carga NP +10.</summary>
public sealed class RainChant() : MorganCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("NpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (Owner.Creature.HasPower<RainWitchFormPower>() || Owner.Creature.HasPower<WinterQueenFormPower>())
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["NpCharge"].UpgradeValueBy(4m);
    }
}
