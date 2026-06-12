using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Sellado del Cilindro — espejo de conversión Estrellas→NP (rediseño v2, reemplaza
/// a ChalkWall en el pool): 0E Habilidad; si tenés 50+ Estrellas de Crítico, perdé 50
/// y ganá 50 de Carga NP. Mejora (parche P5 del juez): exige y consume solo 40.
/// Par exacto de <see cref="CylinderVent"/> — fungibilidad total de los dos medidores.
/// </summary>
public sealed class CylinderSeal() : MashShielderCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StarCost", 50),
        new DynamicVar("NpCharge", 50)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue)) return;
        await CritStars.Gain(Owner.Creature, -DynamicVars["StarCost"].IntValue, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StarCost"].UpgradeValueBy(-10m);
    }
}
