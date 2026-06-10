using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// LORD CHALDEAS — NP card (50 charge): a fortress of Bulwark Block.
/// Overcharge: even taller.
/// </summary>
public sealed class LordChaldeas() : MashShielderCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public const int ChargeCost = 50;
    private const int OverchargeBonus = 12;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(24m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("OverchargeBonus", OverchargeBonus)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var overcharged = NpCharge.IsOvercharged(Owner.Creature);
        await NpCharge.PayForNpCard(Owner.Creature, ChargeCost, this);

        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (overcharged)
        {
            await BlockRetention.GainBulwarkBlock(this, Owner.Creature, DynamicVars["OverchargeBonus"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
    }
}
