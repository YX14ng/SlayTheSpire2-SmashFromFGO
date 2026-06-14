using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Kata de las Mil Estocadas (千突之形, KIT) — DESIGN-OKITA §5.3. 1⚡ Poder: aplica
/// <see cref="ThousandThrustsPower"/> (el 3er Ataque que jugás cada turno: +10★ y +10 Carga NP)
/// (up: +20★). Arquetipo Shukuchi N. Fija StarsGain/NpGainValue del power desde los DynamicVars.
/// </summary>
public sealed class ThousandThrusts() : OkitaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 10), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ThousandThrustsPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.StarsGain = DynamicVars["Stars"].IntValue;
            power.NpGainValue = DynamicVars["NpCharge"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m); // +10★ -> +20★
}
