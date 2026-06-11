using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Llamado de los Caballeros Hada (妖精骑士召集) — 2 de Maldición a TODOS (Barghest),
/// 1 de Débil a TODOS (Baobhan Sith), 6 de Bloqueo (Melusine). Exhaust.
/// </summary>
public sealed class CallOfTheFairyKnights() : MorganCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Curse", 2),
        new PowerVar<WeakPower>("Weak", 1m),
        new BlockVar(6m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CursePower>(), HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
            }
        }
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await PowerCmd.Apply<WeakPower>(enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
            }
        }
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Curse"].UpgradeValueBy(1m);
        DynamicVars["Weak"].UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
