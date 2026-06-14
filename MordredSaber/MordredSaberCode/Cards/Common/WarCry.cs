using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Grito de Guerra (战吼) — DESIGN-MORDRED §5.1. 1⚡ Hab: +20 de Carga NP + 10 Estrellas (up +10/+10).
/// Doble-feeder neutro de las dos economías (el grito de rebelión carga medidor y banco a la vez).
/// Patrón War Cry / Brave Resolve con doble recurso.
/// </summary>
public sealed class WarCry() : MordredCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 20), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
