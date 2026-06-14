using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Boca-Licuadora del Dragón (龙之绞肉口 / Dragon's Blender-Maw) — DESIGN-OBERON §6.4. 3⚡ Ataque: 30 de
/// daño; si Deuda ≥ 3, +10 (la deuda alimenta al gusano) (up 38 / +12). Lee <see cref="DebtPower"/>.
/// Glow con Deuda ≥ 3.
/// </summary>
public sealed class DragonsBlenderMaw() : OberonCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int DebtThreshold = 3;
    private const int DebtBonus = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(30m, ValueProp.Move), new DynamicVar("Bonus", DebtBonus)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) >= DebtThreshold;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = DebtPower.Of(Owner.Creature) >= DebtThreshold ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
