using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Tributo al Trono (王座供奉) — DESIGN-MORDRED §5.1, ESPEJO B. 0⚡ Hab: si tenés ≥50 Estrellas,
/// perdé 50 y ganá 50 de Carga NP (up: consume solo 30). Convierte el banco de ★ en medidor de ulti.
/// Par exacto de <see cref="SpoilsOfCamelot"/>. Patrón CylinderSeal.
/// </summary>
public sealed class TributeToTheThrone() : MordredCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 50), new DynamicVar("NpCharge", 50)];

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

    protected override void OnUpgrade() => DynamicVars["StarCost"].UpgradeValueBy(-20m);
}
