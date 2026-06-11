using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>Meme: Mamá Boba (笨蛋妈妈) — 0⚡: roba 1 + Carga NP 5; mejorada además 2 de Bloqueo.</summary>
public sealed class SillyMama() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DynamicVar("NpCharge", 5),
        new BlockVar(0m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (DynamicVars.Block.BaseValue > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(5m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
