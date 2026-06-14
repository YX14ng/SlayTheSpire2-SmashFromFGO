using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Pagaré del Apocalipsis (末日借据 / Apocalypse IOU) — DESIGN-OBERON §6.4. 1⚡ Ataque: 8 de daño; +4 por
/// cada punto de Deuda que tengas (máx +20) (up 10 / +5 por Deuda). La Deuda como activo, payoff escalar.
/// Lee <see cref="DebtPower"/>. Glow con Deuda > 0.
/// </summary>
public sealed class ApocalypseIOU() : OberonCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int PerDebt = 4;
    private const int MaxBonus = 20;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("PerDebt", PerDebt)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = Math.Min(DebtPower.Of(Owner.Creature) * DynamicVars["PerDebt"].IntValue, MaxBonus);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["PerDebt"].UpgradeValueBy(1m);
    }
}
