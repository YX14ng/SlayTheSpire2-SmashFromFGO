using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Carga de la Cacería (狂猎冲锋) — rediseño 2026-06-15 (swap Estrellas→Maldición): 10 de daño
/// a TODOS + aplica 3 de Maldición a TODOS. El AoE sembrador de la Cacería Salvaje: deja el campo
/// listo para cosechar con la Sentencia o la ventana del NP. (up +4 de daño)
/// </summary>
public sealed class WildHuntCharge() : MorganCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("Curse", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature).Where(e => !e.IsDead).ToList())
        {
            await Curses.Apply(choiceContext, enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
