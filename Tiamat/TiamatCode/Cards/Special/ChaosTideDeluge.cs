using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Cards;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Marea de Caos: Diluvio — carta del MAZO ESPECIAL Bestia (manifestada al abrir la ventana de NP,
/// <see cref="IBeastEphemeral"/>, Exhaust). 0⚡: reabastece el campo con Maldición a TODOS los
/// enemigos para alimentar la mordida doble del enjambre Bestia (que apunta al más maldito).
/// </summary>
public sealed class ChaosTideDeluge() : TiamatCard(0, CardType.Skill, CardRarity.Event, TargetType.AllEnemies), IBeastEphemeral
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Curse", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            await Curses.Apply(choiceContext, enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(1m);
}
