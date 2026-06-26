using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Tributo Abisal — el 2º canje de Maldición (DESIGN-REVIEW-2 §2). Donde Sobremarea cobra la
/// Maldición como Carga NP, este la cobra como DAÑO INMEDIATO: consume hasta !Curse! de Maldición
/// del más maldito e inflige ese tanto (más una base) a esa misma presa. Le da a Lily una forma de
/// VOLCAR el campo de DoT en daño directo sin esperar la ventana Bestia.
/// </summary>
public sealed class AbyssalToll() : TiamatCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Curse", 6)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    // El campo de Maldición debe existir para que el canje rinda; si no, brilla apagado.
    protected override bool ShouldGlowGoldInternal => Curses.MostCursed((CombatState)Owner.Creature.CombatState, Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prey = Curses.MostCursed((CombatState)Owner.Creature.CombatState, Owner.Creature);
        if (prey == null) return;
        var consumed = await Curses.Consume(prey, DynamicVars["Curse"].IntValue);
        var dmg = DynamicVars.Damage.BaseValue + consumed;
        await DamageCmd.Attack(dmg).FromCard(this).Targeting(prey)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(3m);
}
