using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Special;

/// <summary>
/// The ult manifested at 100 NP while in SHIELDER form — Mash's true Noble Phantasm
/// from FGO: LORD CHALDEAS, the wall that protects everything. Pure Bulwark Block.
/// </summary>
public sealed class LordChaldeasUnleashed() : MashShielderCard(0, CardType.Skill, CardRarity.Event, TargetType.Self), IMashNpCard
{
    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(35m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>()];

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
    }
}
