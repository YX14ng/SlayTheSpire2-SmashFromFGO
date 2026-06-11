using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// LORD CHALDEAS — NP card (min 50 charge, consumes ALL): a fortress of Bulwark Block.
/// FGO Overcharge: +Block per 10 charge consumed beyond the minimum.
/// </summary>
public sealed class LordChaldeas() : MashShielderCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public const int ChargeCost = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(24m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        await BlockRetention.GainBulwarkBlock(this, Owner.Creature,
            (BlockVar)DynamicVars.Block, cardPlay);
        if (bonus > 0)
        {
            await BlockRetention.GainBulwarkBlock(this, Owner.Creature, bonus);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
        DynamicVars["PerTen"].UpgradeValueBy(1m);
    }
}
