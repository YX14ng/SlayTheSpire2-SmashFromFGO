using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Última Voluntad (最后的意志, §7) — gran inyección de Carga NP de un saque, con Exhaust como cooldown.
/// Base: 1⚡, +50 NP. Up: cuesta 0 y da +100 (P5: NO hornear 0⚡/+100 en la base — se gana con la mejora).
/// Reusa NpCharge.Gain 100% (cap 300 dentro del helper).
/// </summary>
public sealed class LastWill() : SiegfriedCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Np", 50)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars["Np"].UpgradeValueBy(50m);
    }
}
