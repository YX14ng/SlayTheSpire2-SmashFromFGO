using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Common;

/// <summary>«Mestizo.» (「杂种。」) — DESIGN-GILGAMESH §5.2, el insulto marca registrada. 0⚡ Hab: aplica
/// 1 de Vulnerable a un enemigo + 5 Carga NP (up: 2 / +10). El up sube ambos.</summary>
public sealed class Mongrel() : GilgameshCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<VulnerablePower>("Vulnerable", 1m), new DynamicVar("Np", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
        DynamicVars["Np"].UpgradeValueBy(5m);
    }
}
