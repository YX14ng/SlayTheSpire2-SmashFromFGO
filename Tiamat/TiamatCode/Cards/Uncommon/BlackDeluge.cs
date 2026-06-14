using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>Diluvio Negro — esparcís Maldición a TODOS los enemigos; el enjambre se nutre de
/// cada presa cursada (Crianza por enemigo alcanzado). El AoE de control + sustain del motor.</summary>
public sealed class BlackDeluge() : TiamatCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Curse", 4),
        new DynamicVar("Nurture", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CursePower>(), HoverTipFactory.FromPower<LahmuNurturePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cursed = 0;
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            await Curses.Apply(enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
            cursed++;
        }
        if (cursed > 0)
        {
            await Lahmu.Feed(Owner.Creature, DynamicVars["Nurture"].IntValue * cursed, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(2m);
}
