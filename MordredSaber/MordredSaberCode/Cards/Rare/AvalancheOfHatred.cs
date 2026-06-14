using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Avalancha de Odio (憎恶雪崩) — DESIGN-MORDRED §5.3. 2⚡ At: 4 de daño ×4 a UN enemigo; si consumiste
/// un CRÍTICO este turno, +2 por golpe (up: 5×4), glow. El multi-hit post-crítico: la lluvia que cae
/// tras doblar el golpe. Rider leído con <see cref="CritConsumedThisTurnPower"/> (lo aplica
/// RedLightningChannelPower). El up sube SOLO el daño base por golpe (el rider queda en su denominación).
/// </summary>
public sealed class AvalancheOfHatred() : MordredCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int Hits = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("CritBonus", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritConsumedThisTurnPower>()];

    private bool CritConsumed => Owner.Creature.HasPower<CritConsumedThisTurnPower>();

    protected override bool ShouldGlowGoldInternal => CritConsumed;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = CritConsumed ? DynamicVars["CritBonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).WithHitCount(Hits).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}
