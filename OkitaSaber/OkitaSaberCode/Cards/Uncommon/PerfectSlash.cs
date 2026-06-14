using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Tajo Perfecto (完美一斩) — DESIGN-OKITA §5.3. 2⚡ At: 16 daño; si tenés CRÍTICO LISTO: robá 1 al
/// criticar (up: 20). Blanco natural del ×2 (patrón Witch). El ×2 lo aplica CritReadyPower al golpe;
/// el robo se dispara si había un Crítico Listo en cola al jugarla. Glow cuando hay Crítico Listo.
/// </summary>
public sealed class PerfectSlash() : OkitaCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int Draw = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    private bool HasCritReady => Owner.Creature.GetPowerAmount<CritReadyPower>() > 0;

    protected override bool ShouldGlowGoldInternal => HasCritReady;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var crit = HasCritReady;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
        if (crit) await CardPileCmd.Draw(choiceContext, Draw, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m); // 16 -> 20
}
