using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Sobremarea — el canje de Maldición a Sobrecarga (REDESIGN-TIAMAT §49, el punto 4 del puente).
/// Consume hasta !Curse! de Maldición del enemigo más maldito y lo convertís en !NpCharge! de
/// Carga NP y !Nurture! de Crianza. La marea desborda: el campo de Maldición se cobra como
/// combustible de la ventana Génesis. Solo paga si hay un campo ya sembrado.
/// </summary>
public sealed class Overtide() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Curse", 6),
        new DynamicVar("NpCharge", 18),
        new DynamicVar("Nurture", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CursePower>(),
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<LahmuNurturePower>()
    ];

    // El campo de Maldición debe existir para que el canje rinda; si no, brilla apagado.
    protected override bool ShouldGlowGoldInternal => Curses.MostCursed((CombatState)Owner.Creature.CombatState, Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prey = Curses.MostCursed((CombatState)Owner.Creature.CombatState, Owner.Creature);
        if (prey == null) return;
        var consumed = await Curses.Consume(prey, DynamicVars["Curse"].IntValue);
        if (consumed <= 0) return;
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await Lahmu.Feed(Owner.Creature, DynamicVars["Nurture"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(6m);
}
