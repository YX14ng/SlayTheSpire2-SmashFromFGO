using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Protección del Lago (湖之加护) — Morgan S2: Carga NP +20; en forma Bruja/Invierno:
/// roba 1. Rediseño v2: glow dorado en la forma correcta. (up +8)
/// </summary>
public sealed class ProtectionOfTheLake() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.HasPower<RainWitchFormPower>() || Owner.Creature.HasPower<WinterQueenFormPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (Owner.Creature.HasPower<RainWitchFormPower>() || Owner.Creature.HasPower<WinterQueenFormPower>())
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(8m);
    }
}
