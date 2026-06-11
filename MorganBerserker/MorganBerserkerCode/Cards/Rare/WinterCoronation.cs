using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Coronación del Invierno (冬之女王戴冠) — enter the permanent Winter Queen form
/// (both reigns at once; no way back) and apply 3 Curse to ALL enemies.
/// </summary>
public sealed class WinterCoronation() : MorganCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Curse", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await FormSwitch.Enter<WinterQueenFormPower>(choiceContext, Owner.Creature, this);
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
        EnergyCost.UpgradeBy(-1);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
