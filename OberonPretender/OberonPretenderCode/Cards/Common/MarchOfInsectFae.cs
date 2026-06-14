using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Marcha de las Hadas-Insecto (虫之妖精的行军 / March of the Insect-Fae) — DESIGN-OBERON §6.2.
/// 1⚡ Ataque: 4 de daño a TODOS los enemigos; +10 Carga NP (up 6). El barrido AoE (−30% por el rider
/// + el reparto) que carga el medidor contra grupos — el ejército de insectos del Lostbelt. El up
/// sube SOLO el daño; el NP queda en su denominación.
/// </summary>
public sealed class MarchOfInsectFae() : OberonCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
