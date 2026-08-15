using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #2 Saeta Maldita (诅咒之矢) — 4 de daño + 2 de Maldición a TODOS los enemigos. Re-perfilado
/// 2026-06-25 (P2): era un clon mecánico del básico QuickMorgan (1⚡, 6 daño + 2 Maldición a UN
/// enemigo); ahora es el sembrador de Maldición en ABANICO que diferencia el común del básico
/// single-target — el motor que la Reina cosecha escala mejor con varios objetivos malditos.
/// </summary>
public sealed class CursedBolt() : MorganCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Curse", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(choiceContext, enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
