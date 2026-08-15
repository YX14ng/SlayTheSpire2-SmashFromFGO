using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Ira de la Tormenta (风暴之怒) — re-efecto RE-POOL V2 (§5.2): de 3⚡/26 plano a payoff de la
/// danza: 1⚡, 8 de daño; si cambiaste de forma este turno: +8 y aplicá 3 de Maldición
/// (mejora 10/+10/4). Flag = bit 9 del estado de turno (cetro/Ancient). Glow con el flag.
/// </summary>
public sealed class StormsWrath() : MorganCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Bonus", 8),
        new DynamicVar("Curse", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    private bool ShiftedThisTurn => FgoCombatState.GetTurn(Owner.Creature, 9) != 0;

    protected override bool ShouldGlowGoldInternal => ShiftedThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var shifted = ShiftedThisTurn;
        var damage = DynamicVars.Damage.BaseValue;
        if (shifted)
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
        if (shifted && !cardPlay.Target.IsDead)
        {
            await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
