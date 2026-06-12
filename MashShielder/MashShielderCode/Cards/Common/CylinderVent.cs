using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Purga del Cilindro — espejo de conversión NP→Estrellas (rediseño v2, reemplaza
/// a Skirmish en el pool): 0E Habilidad; si tenés 50+ de Carga NP, perdé 50 y ganá
/// 50 Estrellas de Crítico. Mejora (parche P5 del juez): exige y consume solo 40
/// (ganancia neta +10 por ciclo, no +20). Par exacto de <see cref="CylinderSeal"/>:
/// con ambos en común a 0E, ningún contador sobra nunca.
/// </summary>
public sealed class CylinderVent() : MashShielderCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCost", 50),
        new DynamicVar("Stars", 50)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["NpCost"].IntValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await NpCharge.Spend(Owner.Creature, DynamicVars["NpCost"].IntValue, this)) return;
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCost"].UpgradeValueBy(-10m);
    }
}
