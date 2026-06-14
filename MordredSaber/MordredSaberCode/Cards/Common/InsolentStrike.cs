using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Espadazo Insolente (无礼一击) — DESIGN-MORDRED §5.1. 0⚡ At: 4 de daño; si CONSUMISTE un Crítico
/// este turno, +10 NP (up +2/+10), glow. El rider calibrado al motor ★→×2→NP: leído con el marcador
/// <see cref="CritConsumedThisTurnPower"/> (lo aplica RedLightningChannelPower). Patrón ShieldRam (lectura
/// del estado de crítico). El daño 0⚡ pequeño justifica el +NP condicional cuando el crítico ya cayó.
/// </summary>
public sealed class InsolentStrike() : MordredCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    private bool CritConsumed => Owner.Creature.HasPower<CritConsumedThisTurnPower>();

    protected override bool ShouldGlowGoldInternal => CritConsumed;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var consumed = CritConsumed;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (consumed)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
