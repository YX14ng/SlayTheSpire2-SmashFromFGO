using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Common;

/// <summary>Marea Creciente — siembra de Maldición en AoE (la marea sube y todo se mancha de lodo).
/// El puente, esparcido: prepara el campo para que el enjambre y la ventana Bestia cosechen en masa.</summary>
public sealed class RisingTide() : TiamatCard(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Curse", 4)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            await Curses.Apply(choiceContext, enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(2m);
}
