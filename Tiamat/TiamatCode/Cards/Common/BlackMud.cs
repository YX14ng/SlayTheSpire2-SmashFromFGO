using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Common;

/// <summary>Lodo Negro — sembradora de Maldición de coste 0 (el lodo del Mar de Vida que mancha la
/// presa). Acuña barato la divisa que la Bestia gasta; sin tope de manos por ser gratis.</summary>
public sealed class BlackMud() : TiamatCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Curse", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(2m);
}
