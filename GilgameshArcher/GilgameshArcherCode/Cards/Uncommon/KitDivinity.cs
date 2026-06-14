using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Divinidad B (神性 B) — la pasiva real (Damage Plus). 2⚡ Poder: aplica
/// <see cref="DivinityPower"/> con Amount 2 (tus Ataques hacen +2 de daño, incluidas las Armas del
/// Tesoro; aditivo). up 2→3.</summary>
public sealed class KitDivinity() : GilgameshCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<DivinityPower>("Divinity", 2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DivinityPower>(Owner.Creature, DynamicVars["Divinity"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Divinity"].UpgradeValueBy(1m);
}
