using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>#13 Marca de la Bruja (魔女的印记) — 0⚡: aplica 3 de Maldición.</summary>
public sealed class WitchsMark() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Curse", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Curses.Apply(cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Curse"].UpgradeValueBy(2m);
    }
}
