using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Aceleración de Homúnculo (人造人加速) — DESIGN-MORDRED §5.2. 1⚡ Poder: la 1ª vez por turno que
/// CONSUMÍS un Crítico, +10 Estrellas (up +20). Cierra el motor ★→×2→NP→★, capeado 1/turno (P3).
/// Aplica <see cref="HomunculusAccelerationPower"/> (ICritConsumedListener, lo dispara
/// RedLightningChannelPower); el valor por activación es campo settable que la carta fija desde su
/// DynamicVar. Patrón CrownOfLightning/MemoryOfTrifas (poder con campo settable).
/// </summary>
public sealed class HomunculusAcceleration() : MordredCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<HomunculusAccelerationPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.StarsPerTurn = DynamicVars["Stars"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
