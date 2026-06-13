using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SiegfriedSaber.SiegfriedSaberCode.Cards;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// El Peso de las Expectativas (§7) — el bruiser paciente como carta. Aplica
/// <see cref="WeightOfExpectationsPower"/>: al fin de un turno SIN Ataques, +NP y +1 SdD (tope 2/turno,
/// P3). Auto-limitada: la ult (Ataque) no la deja procar el turno que la usás. El up sube el NP/trigger
/// (20→30), no el tope ni el SdD (conserva el techo P3). Fija NpPerTrigger desde el DynamicVar al aplicar.
/// </summary>
public sealed class WeightOfExpectations() : SiegfriedCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpGain", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<WeightOfExpectationsPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.NpPerTrigger = DynamicVars["NpGain"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["NpGain"].UpgradeValueBy(10m);
}
