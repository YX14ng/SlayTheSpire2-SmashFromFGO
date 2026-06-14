using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Common;

/// <summary>Avaricia del Coleccionista (收藏家之贪) — DESIGN-GILGAMESH §5.2, conversor 等价交换. 0⚡ Hab:
/// perdé 50 Estrellas de Crítico → ganá 50 Carga NP (up: consume 30). Glow cuando pagable. El up baja
/// SOLO el costo de Estrellas (−20). Patrón BattleInstinct (IsPlayable = CritStars.CanPay,
/// ShouldGlowGoldInternal, early-return, CritStars.Gain(-costo)).</summary>
public sealed class CollectorsGreed() : GilgameshCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 50), new DynamicVar("Np", 50)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue)) return;
        await CritStars.Gain(Owner.Creature, -DynamicVars["StarCost"].IntValue, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["StarCost"].UpgradeValueBy(-20m);
}
