using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #11 Velo de Niebla (雾之帷幕) — rediseño v2: espejo A de conversión (par 等价交换
/// de Jeanne): 0⚡, si tenés ≥50 de Carga NP, perdé 50 y obtené 50 Estrellas de Crítico
/// (el maná se dispersa en niebla refulgente). Glow cuando pagable.
/// (up: consume 40 — parche P5 del juez, neto +10; NO bajar a 30 sin playtest con P1)
/// </summary>
public sealed class MistVeil() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ChargeCost", 50),
        new DynamicVar("Stars", 50)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<FGOCore.FGOCoreCode.Stars.CritStarsPower>()
    ];

    // Carga REAL, sin waiver: los espejos no son cartas NP y no deben quemar el comodín.
    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["ChargeCost"].IntValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await NpCharge.Spend(Owner.Creature, DynamicVars["ChargeCost"].IntValue, this)) return;
        await FGOCore.FGOCoreCode.Stars.CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ChargeCost"].UpgradeValueBy(-10m);
    }
}
