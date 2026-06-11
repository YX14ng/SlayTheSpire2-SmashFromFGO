using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Ejecución Real (女王处刑) — 14 de daño; si mata: Carga NP +25 y 2 de Maldición a TODOS.</summary>
public sealed class RoyalExecution() : MorganCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new DynamicVar("NpCharge", 25),
        new DynamicVar("Curse", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
        if (cardPlay.Target.IsDead)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
            foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
            {
                if (!enemy.IsDead)
                {
                    await Curses.Apply(enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["NpCharge"].UpgradeValueBy(5m);
    }
}
