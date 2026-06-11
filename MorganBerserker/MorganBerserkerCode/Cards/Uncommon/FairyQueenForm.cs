using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Cambio: La Reina (切换：妖精女王) — enter Fairy Queen form + 3 Curse.</summary>
public sealed class FairyQueenForm() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Curse", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await FormSwitch.Enter<FairyQueenFormPower>(choiceContext, Owner.Creature, this);
        await Curses.Apply(cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Curse"].UpgradeValueBy(2m);
    }
}
