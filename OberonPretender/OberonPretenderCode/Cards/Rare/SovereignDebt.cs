using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Stars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Deuda Soberana (主权债务 / Sovereign Debt) — DESIGN-OBERON §6.4, la carta-combo modelo (patrón 黑闪).
/// 1⚡ Ataque · PRÉSTAMO: +100 Estrellas (dispara el umbral → CRÍTICO LISTO), LUEGO 12 de daño (sale ×2
/// porque <see cref="CritReadyPower"/> lo reclama). Deuda 4 (up 16). <see cref="ILoanCard"/> → crédito
/// cortado a Deuda ≥ 5.
/// </summary>
public sealed class SovereignDebt() : OberonCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), ILoanCard
{
    public const int Debt = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Stars", 100),
        new DamageVar(12m, ValueProp.Move),
        new DynamicVar("Debt", Debt)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Las estrellas cruzan el umbral (auto-payoff → CRÍTICO LISTO) ANTES del golpe, que lo consume.
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
