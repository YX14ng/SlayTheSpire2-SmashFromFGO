using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Espina del Abismo (深渊之棘 / Thorn of the Abyss) — DESIGN-OBERON §6.3. 2⚡ Ataque · PRÉSTAMO: 18 de
/// daño. Deuda 2 (up 24). 18 a 2⚡ = baseline 'con downside'; el downside es el pagaré.
/// <see cref="ILoanCard"/> → endulzante del Rey + «Interés a Mi Favor»; crédito cortado a Deuda ≥ 5.
/// </summary>
public sealed class ThornOfTheAbyss() : OberonCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), ILoanCard
{
    public const int Debt = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(18m, ValueProp.Move), new DynamicVar("Debt", Debt)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

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

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}
