using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Stars;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Cabalgar la Polilla Halcón (骑乘鹰蛾 / Ride the Hawk Moth) — DESIGN-OBERON §6.2. 1⚡ Habilidad:
/// robá 1; +20 Estrellas (Riding A: 130 km/h cuando nadie mira). El up sube SOLO el robo (a 2; las ★
/// quedan en su denominación). Reusa CritStarsPower de FGOCore.
/// </summary>
public sealed class RideTheHawkMoth() : OberonCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Draw", 1), new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}
