using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>Diluvio del Génesis — el pico de daño AoE de Lily que COBRA el campo sembrado: pega a
/// TODOS y suma +1 por cada 2 de Maldición que cargue cada objetivo (por objetivo, sin tocar la
/// Maldición: la cobra sin consumirla, a diferencia de Tributo Abisal). Base 8 AoE a 2⚡ = tasa AoE
/// con descuento (§2), el exceso viene del setup previo (§3: condición que puede fallar). También
/// suma al hueco Rara/Ataque=2 que rompía el evento de pociones.</summary>
public sealed class GenesisDeluge() : TiamatCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("CursePerBonus", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            var dmg = DynamicVars.Damage.BaseValue + Curses.Of(enemy) / DynamicVars["CursePerBonus"].IntValue;
            await DamageCmd.Attack(dmg).FromCard(this).Targeting(enemy)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
