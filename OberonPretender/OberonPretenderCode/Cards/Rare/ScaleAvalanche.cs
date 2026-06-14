using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Stars;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Avalancha de Escamas (鳞雪崩 / Scale Avalanche) — DESIGN-OBERON §6.4. 2⚡ Ataque: 7 de daño ×3 a un
/// enemigo; con CRÍTICO LISTO los tres golpes critican (up 9×3). El <see cref="CritReadyPower"/> de
/// FGOCore ya dobla TODOS los golpes de la carta y consume 1 stack — no requiere TryConsume. Glow con
/// Crítico Listo.
/// </summary>
public sealed class ScaleAvalanche() : OberonCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int Hits = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.GetPowerAmount<CritReadyPower>() > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
