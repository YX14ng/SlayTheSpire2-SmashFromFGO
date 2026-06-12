using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #19 Acero del Invierno (寒冬之铁) — rediseño v2: 2⚡, 13 de Bloqueo; si algún
/// enemigo tiene Maldición: +5 de Bloqueo (Maldición→Bloqueo). Glow dorado. (up +4/+2)
/// </summary>
public sealed class WinterSteel() : MorganCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(13m, ValueProp.Move),
        new DynamicVar("Bonus", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override bool ShouldGlowGoldInternal =>
        Curses.CursedEnemies(Owner.Creature.CombatState, Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (Curses.CursedEnemies(Owner.Creature.CombatState, Owner.Creature) > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Bonus"].BaseValue, ValueProp.Move, null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
