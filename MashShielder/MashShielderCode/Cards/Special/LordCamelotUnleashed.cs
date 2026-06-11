using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Special;

/// <summary>
/// The ult: generated for free (into hand, cost 0) the first time the NP gauge
/// reaches 100 in a combat. Playing it consumes the full gauge.
/// </summary>
public sealed class LordCamelotUnleashed() : MashShielderCard(0, CardType.Skill, CardRarity.Event, TargetType.Self)
{
    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(30m, ValueProp.Move),
        new PowerVar<StrengthPower>("Strength", 3m),
        new PowerVar<ProvokePower>("Provoke", 12m),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<ProvokePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);

        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        // NP level (dupes): +15% per level, added as flat extra Bulwark Block.
        var npBonus = NpLevels.Scale(Owner, DynamicVars.Block.BaseValue) - DynamicVars.Block.BaseValue;
        if (npBonus > 0)
        {
            await BlockRetention.GainBulwarkBlock(this, Owner.Creature, npBonus);
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
