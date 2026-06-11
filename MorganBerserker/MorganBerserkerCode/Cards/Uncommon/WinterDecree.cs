using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Decreto de Invierno (寒冬敕令) — 12 de Bloqueo + 2 de Maldición a TODOS.</summary>
public sealed class WinterDecree() : MorganCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new DynamicVar("Curse", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
