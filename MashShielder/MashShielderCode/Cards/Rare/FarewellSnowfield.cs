using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Campo de Nieve del Adiós — a great farewell wall + NP Charge. Exhaust.
/// Rediseño v2: 20 Bloqueo con Baluarte (up +6) + 20 NP (up +10; antes +5 — denominación).
/// </summary>
public sealed class FarewellSnowfield() : MashShielderCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(20m, ValueProp.Move),
        new DynamicVar("NpCharge", 20)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
