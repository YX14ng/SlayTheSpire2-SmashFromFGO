using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Trono del Observador (旁观者之座) — la arrogancia como motor; la presión lo interrumpe. 1⚡
/// Poder: aplica <see cref="ThroneOfTheOnlookerPower"/>. Al final de tu turno, si NO jugaste ninguna
/// carta de Bloqueo (Owner.Block==0): +10 Estrellas y +10 Carga NP (up 15 / 15). Fija los campos
/// Stars/Np del power desde los DynamicVars al aplicar (patrón WeightOfExpectations).</summary>
public sealed class ThroneOfTheOnlooker() : GilgameshCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 10), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ThroneOfTheOnlookerPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.Stars = DynamicVars["Stars"].IntValue;
            power.Np = DynamicVars["Np"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(5m);
        DynamicVars["Np"].UpgradeValueBy(5m);
    }
}
