using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Common;

/// <summary>Andanada de Portales (传送门连射) — DESIGN-GILGAMESH §5.2, rider calibrado a la starter. 1⚡ At:
/// 9 de daño; si jugaste un Arma del Tesoro este turno: +3 (up 12 / +4). Glow cuando jugaste un Arma.
/// Lee <see cref="ArmsPlayedPower"/> (0 hasta que el Arsenal lo alimente). Patrón DragonSlayerStrike
/// (base + bonus condicional plano). El up sube ambos.</summary>
public sealed class PortalVolley() : GilgameshCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("Bonus", 3)];

    protected override bool ShouldGlowGoldInternal => ArmsPlayedPower.PlayedThisTurn(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = ArmsPlayedPower.PlayedThisTurn(Owner.Creature) > 0 ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}
