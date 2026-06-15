using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Asalto del Tesoro Abierto (宝库强袭) — payoff de Armas. 1⚡ At: 6 de daño; +3 por cada Arma
/// del Tesoro jugada este turno. Glow cuando jugaste un Arma. up: base 6→8, por-Arma 3→4. Lee
/// <see cref="ArmsPlayedPower"/> (0 hasta que el Arsenal lo alimente).</summary>
public sealed class OpenTreasuryAssault() : GilgameshCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("PerArm", 3)];

    protected override bool ShouldGlowGoldInternal => ArmsPlayedPower.PlayedThisTurn(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = ArmsPlayedPower.PlayedThisTurn(Owner.Creature) * DynamicVars["PerArm"].IntValue;
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue + bonus);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["PerArm"].UpgradeValueBy(1m);
    }
}
