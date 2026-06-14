using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Euforia Nocturna (夜之欢愉 / Nocturnal Euphoria) — DESIGN-OBERON §6.3. 1⚡ Poder: al final del
/// turno, si no te queda Deuda impaga, +10 Carga NP y +10 Estrellas (premia la solvencia, lore S1).
/// Aplica <see cref="NocturnalEuphoriaPower"/> y le fija los riders (el up sube ambos a 15).
/// </summary>
public sealed class NocturnalEuphoria() : OberonCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Charge", 10), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<NocturnalEuphoriaPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.Charge = DynamicVars["Charge"].IntValue;
            power.Stars = DynamicVars["Stars"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Charge"].UpgradeValueBy(5m);
        DynamicVars["Stars"].UpgradeValueBy(5m);
    }
}
