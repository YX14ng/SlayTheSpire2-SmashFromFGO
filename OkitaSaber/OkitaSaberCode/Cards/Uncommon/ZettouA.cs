using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Zettou A (绝刀A, KIT) — DESIGN-OKITA §5.3. 1⚡ Hab, Exhaust: +30 Carga NP; +30★; este turno tus
/// CRÍTICOS hacen +6 (<see cref="GloryEdgePower"/>) (up: +40 / +40 / +8). El turno de gloria
/// embotellado (star gather + crit dmg +50% + NP 30%).
/// </summary>
public sealed class ZettouA() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 30),
        new DynamicVar("Stars", 30),
        new PowerVar<GloryEdgePower>("GloryEdge", 6m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<CritReadyPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await PowerCmd.Apply<GloryEdgePower>(Owner.Creature, DynamicVars["GloryEdge"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m); // +30 -> +40
        DynamicVars["Stars"].UpgradeValueBy(10m);    // +30 -> +40
        DynamicVars["GloryEdge"].UpgradeValueBy(2m);  // +6 -> +8
    }
}
