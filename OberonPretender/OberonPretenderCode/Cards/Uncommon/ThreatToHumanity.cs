using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Amenaza para la Humanidad (人类之威胁 / Threat to Humanity) — DESIGN-OBERON §6.3, el rasgo Beast.
/// 2⚡ Poder: cuando un enemigo muere por tu mano, +20 Carga NP y +20 Estrellas (up +30/+30). Aplica
/// <see cref="ThreatToHumanityPower"/> y le fija los riders.
/// </summary>
public sealed class ThreatToHumanity() : OberonCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Charge", 20), new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ThreatToHumanityPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.Charge = DynamicVars["Charge"].IntValue;
            power.Stars = DynamicVars["Stars"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Charge"].UpgradeValueBy(10m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
