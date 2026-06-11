using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Impuesto Extraordinario (临时增税) — 4 de Maldición a TODOS; cura 2 HP por
/// cada enemigo con Maldición. Exhaust.
/// </summary>
public sealed class ExtraordinaryTax() : MorganCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Curse", 4),
        new HealVar(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
            }
        }

        var cursed = Curses.CursedEnemies(Owner.Creature.CombatState, Owner.Creature);
        if (cursed > 0)
        {
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue * cursed);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Curse"].UpgradeValueBy(2m);
        DynamicVars.Heal.UpgradeValueBy(1m);
    }
}
