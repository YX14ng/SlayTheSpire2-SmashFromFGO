using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// LORD CAMELOT — NP card (min 70 charge, consumes ALL): the Castle of the Distant Utopia.
/// FGO Overcharge: +Block per 10 extra charge. Strength and Intercept always included.
/// </summary>
public sealed class LordCamelot() : MashShielderCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public const int ChargeCost = 70;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(23m, ValueProp.Move),
        new PowerVar<StrengthPower>("Strength", 3m),
        new PowerVar<ProvokePower>("Provoke", 12m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<ProvokePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (bonus > 0)
        {
            await BlockRetention.GainBulwarkBlock(this, Owner.Creature, bonus);
        }
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<ProvokePower>(Owner.Creature, DynamicVars["Provoke"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(10m);
        DynamicVars["Strength"].UpgradeValueBy(1m);
    }
}
