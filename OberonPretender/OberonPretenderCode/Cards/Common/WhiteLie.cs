using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Mentira Piadosa (善意的谎言 / White Lie) — DESIGN-OBERON §6.2. 1⚡ Ataque · PRÉSTAMO: 12 de daño
/// (up 16). Deuda 1 (= +10 NP al cobro / 3 HP si impago). La sobre-tasa común (12 a 1⚡, ≈ ½⚡ extra)
/// la paga el pagaré: el golpe limpio AHORA, la cuenta al amanecer. <see cref="ILoanCard"/> →
/// la dispara la pasiva del Rey (1er préstamo del turno = endulzado) y el «Interés a Mi Favor»;
/// el límite de crédito la apaga (glow off + no jugable) a Deuda ≥ 5.
/// </summary>
public sealed class WhiteLie() : OberonCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), ILoanCard
{
    public const int Debt = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, ValueProp.Move), new DynamicVar("Debt", Debt)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    // Límite de crédito (§3.c): a Deuda >= 5 el préstamo deja de poder jugarse y su glow se apaga.
    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
