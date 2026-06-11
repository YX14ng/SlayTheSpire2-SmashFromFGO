using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Cosecha de Maldiciones (诅咒收割) — duplica la Maldición de UN enemigo (máx. +6).</summary>
public sealed class CurseHarvest() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Max", 6)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override bool IsPlayable => true;

    protected override bool ShouldGlowGoldInternal =>
        Curses.MostCursed(Owner.Creature.CombatState, Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var toAdd = Math.Min(Curses.Of(cardPlay.Target), DynamicVars["Max"].IntValue);
        if (toAdd <= 0) return;

        await Curses.Apply(cardPlay.Target, toAdd, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Max"].UpgradeValueBy(4m);
    }
}
