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
public sealed class LordCamelotUnleashed() : MashShielderCard(0, CardType.Skill, CardRarity.Event, TargetType.Self), IMashNpCard
{
    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(30m, ValueProp.Move),
        new PowerVar<StrengthPower>("Strength", 3m),
        new PowerVar<ProvokePower>("Provoke", 12m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<ProvokePower>()];

    // Pasar la carta: el waiver de Pioneer NO cubre Event (parche P3) — sin él,
    // CanPay daría glow/playable falsos con el medidor vacío y un waiver activo.
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        // NP level (dupes): +15% per level over the full amount, added as flat extra.
        var total = DynamicVars.Block.BaseValue + bonus;
        var extra = bonus + NpLevels.Scale(Owner, total) - total;

        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (extra > 0)
        {
            await BlockRetention.GainBulwarkBlock(this, Owner.Creature, extra);
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
