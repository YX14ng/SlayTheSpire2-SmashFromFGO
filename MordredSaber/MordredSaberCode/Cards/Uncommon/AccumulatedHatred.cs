using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Odio Acumulado (积怨) — DESIGN-MORDRED §5.2. 1⚡ Poder: cada vez que perdés Vida +10 de Carga NP
/// (máx 2/turno; up: máx 3). Cada sangrado paga DOS economías con la starter (★ por la reliquia, NP
/// por este poder). Aplica <see cref="AccumulatedHatredPower"/>; el tope por turno es campo settable
/// que la carta fija desde su DynamicVar. Patrón WeightOfExpectations (campo settable + tope P3).
/// </summary>
public sealed class AccumulatedHatred() : MordredCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxProcs", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<AccumulatedHatredPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.MaxProcsPerTurn = DynamicVars["MaxProcs"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["MaxProcs"].UpgradeValueBy(1m);
}
