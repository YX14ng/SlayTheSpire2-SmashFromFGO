using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #20 Vasallaje (臣从之礼) — rediseño v2: espejo B de conversión (par 等价交换 de
/// Jeanne): 0⚡, si tenés ≥50 Estrellas de Crítico, perdé 50 y obtené 50 de Carga NP
/// (los vasallos tributan a la Corona). Glow cuando pagable.
/// (up: consume 40 — parche P5 del juez, neto +10; NO bajar a 30 sin playtest con P1)
/// </summary>
public sealed class Vassalage() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StarCost", 50),
        new DynamicVar("NpCharge", 50)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FGOCore.FGOCoreCode.Stars.CritStarsPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override bool IsPlayable =>
        FGOCore.FGOCoreCode.Stars.CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cost = DynamicVars["StarCost"].IntValue;
        if (!FGOCore.FGOCoreCode.Stars.CritStars.CanPay(Owner.Creature, cost)) return;
        await FGOCore.FGOCoreCode.Stars.CritStars.Gain(Owner.Creature, -cost, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StarCost"].UpgradeValueBy(-10m);
    }
}
