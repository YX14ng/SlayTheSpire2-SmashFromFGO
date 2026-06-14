using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Common;

/// <summary>Lluvia de Hojas (刃之雨) — DESIGN-GILGAMESH §5.2. 1⚡ At: 3 de daño ×3 a enemigos aleatorios
/// + 10 Carga NP (up: 4×3). Multi-hit disperso (suma = single §2). El up sube SOLO el daño por golpe;
/// el NP queda fijo. Patrón LightningSplinters (TargetType.RandomEnemy + TargetingRandomOpponents).</summary>
public sealed class RainOfBlades() : GilgameshCard(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
{
    private const int Hits = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits).FromCard(this)
            .TargetingRandomOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}
