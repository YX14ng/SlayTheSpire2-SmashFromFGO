using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #5 Barrido de la Tirana (暴君横扫) — rediseño v2 (parche P3 del juez): 6 de daño a
/// TODOS + 1 por cada 4 de Maldición de cada objetivo (individual, máx +6 → a cap 25
/// = 12 AoE, dentro del techo ×1.5). El payoff escalar de Maldición en común. (up: 6→9)
/// </summary>
public sealed class TyrantsSweep() : MorganCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("CursePer", 4),
        new DynamicVar("MaxBonus", 6)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override bool ShouldGlowGoldInternal =>
        Curses.MostCursed(Owner.Creature.CombatState, Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            var bonus = Math.Min(
                Curses.Of(enemy) / DynamicVars["CursePer"].IntValue,
                DynamicVars["MaxBonus"].IntValue);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(enemy)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
