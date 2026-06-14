using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Forms;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Doble Cara (两面三刀 / Two-Faced) — DESIGN-OBERON §6.3. 1⚡ Ataque: 6 de daño; si cambiaste de forma
/// este combate, +6 (up 8 / +8). El payoff del baile de formas (lee <see cref="FormShiftedPower"/>).
/// Glow tras un cambio de forma.
/// </summary>
public sealed class TwoFaced() : OberonCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int ShiftBonus = 6;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Bonus", ShiftBonus)];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<FormShiftedPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = Owner.Creature.HasPower<FormShiftedPower>() ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
