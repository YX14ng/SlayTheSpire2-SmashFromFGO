using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Botín de Camelot (卡美洛战利品) — DESIGN-MORDRED §5.1, ESPEJO A. 0⚡ Hab: si tenés ≥50 de Carga NP,
/// perdé 50 y ganá 50 Estrellas (up: consume solo 30). Convierte el banco de NP en munición de ★.
/// Par exacto de <see cref="TributeToTheThrone"/> (fungibilidad de los dos medidores). Patrón CylinderVent.
/// </summary>
public sealed class SpoilsOfCamelot() : MordredCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCost", 50), new DynamicVar("Stars", 50)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["NpCost"].IntValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await NpCharge.Spend(Owner.Creature, DynamicVars["NpCost"].IntValue, this)) return;
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCost"].UpgradeValueBy(-20m);
}
