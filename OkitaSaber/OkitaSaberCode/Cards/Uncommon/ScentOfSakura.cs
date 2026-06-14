using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Aroma a Sakura (樱花之香) — DESIGN-OKITA §5.3. 1⚡ Hab: +20★ y +10 Carga NP (up: +30★ / +20).
/// Paquete doble.
/// </summary>
public sealed class ScentOfSakura() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 20), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(10m);    // +20 -> +30
        DynamicVars["NpCharge"].UpgradeValueBy(10m); // +10 -> +20
    }
}
