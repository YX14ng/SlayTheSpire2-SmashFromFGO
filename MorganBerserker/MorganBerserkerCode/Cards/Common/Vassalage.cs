using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #20 Vasallaje (臣从之礼) — rediseño 2026-06-15 (swap Estrellas→Maldición): espejo B del par
/// NP↔Maldición: 0⚡, consume hasta 8 de Maldición del enemigo más maldito y obtené 4 de Carga NP
/// por punto consumido (los vasallos tributan su maldición a la Corona como maná). Glow si hay
/// campo sembrado. (up: consume hasta 12)
/// </summary>
public sealed class Vassalage() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Curse", 8),
        new DynamicVar("NpPerPoint", 4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CursePower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override bool ShouldGlowGoldInternal =>
        Curses.MostCursed((CombatState)Owner.Creature.CombatState, Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prey = Curses.MostCursed((CombatState)Owner.Creature.CombatState, Owner.Creature);
        if (prey == null) return;
        var consumed = await Curses.Consume(prey, DynamicVars["Curse"].IntValue);
        if (consumed <= 0) return;
        await NpCharge.Gain(Owner.Creature, consumed * DynamicVars["NpPerPoint"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Curse"].UpgradeValueBy(4m);
    }
}
