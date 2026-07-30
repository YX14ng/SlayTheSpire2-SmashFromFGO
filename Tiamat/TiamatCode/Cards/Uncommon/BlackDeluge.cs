using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>Diluvio Negro — esparcís Maldición a TODOS los enemigos; el enjambre se nutre de las
/// presas que YA cargaban Maldición (Crianza por enemigo ya maldito, antes de este diluvio). El
/// AoE de control que premia un campo ya sembrado (REDESIGN-TIAMAT §45).</summary>
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
        var enemies = Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature).Where(e => !e.IsDead).ToList();
        // "Por enemigo YA maldito": contar ANTES de esparcir el diluvio (el campo previo, no este).
        var alreadyCursed = enemies.Count(e => Curses.Of(e) > 0);
        foreach (var enemy in enemies)
        {
            await Curses.Apply(choiceContext, enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
        if (alreadyCursed > 0)
        {
            await Lahmu.Feed(choiceContext, Owner.Creature, DynamicVars["Nurture"].IntValue * alreadyCursed, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(2m);
}
