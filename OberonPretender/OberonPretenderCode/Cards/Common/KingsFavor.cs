using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Favor del Rey (国王的恩宠 / King's Favor) — DESIGN-OBERON §6.2. 1⚡ Ataque: 8 de daño; si tenés
/// Deuda: +<see cref="DebtBonus"/> (up 11 / +5 al rider). El lector de Deuda calibrado: la firma del
/// mazo inicial siempre lo enciende, así que el rider es fiable sin ser gratis. Lee
/// <c>DebtPower.Of > 0</c>. Glow si tenés Deuda.
/// </summary>
public sealed class KingsFavor() : OberonCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int DebtBonus = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("Bonus", DebtBonus)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = DebtPower.Of(Owner.Creature) > 0 ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}
