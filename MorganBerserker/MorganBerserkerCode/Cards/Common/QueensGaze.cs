using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Mirada de la Reina (女王的凝视) — re-efecto RE-POOL V2 (§5.1): 1⚡, 2 de Débil + 3 de
/// Maldición (mejora 3/4). Control común del arquetipo B (análogo Sucker Punch); el rider de NP
/// se mudó a las cartas C.
/// </summary>
public sealed class QueensGaze() : MorganCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>("Weak", 2m),
        new DynamicVar("Curse", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
