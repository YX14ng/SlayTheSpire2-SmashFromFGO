using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Cobro Final (最后的清算) — the Tyranny detonator: consumes ALL the target's
/// Curse and deals 2 damage per point (forfeiting the deferred damage).
/// </summary>
public sealed class FinalCollection() : MorganCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(0m, ValueProp.Move),
        new DynamicVar("PerPoint", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override bool IsPlayable => true;

    protected override bool ShouldGlowGoldInternal =>
        Curses.MostCursed(Owner.Creature.CombatState, Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var consumed = await Curses.Consume(cardPlay.Target, CursePower.MaxPerEnemy);
        if (consumed <= 0) return;

        await DamageCmd.Attack(consumed * DynamicVars["PerPoint"].IntValue).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PerPoint"].UpgradeValueBy(1m);
    }
}
