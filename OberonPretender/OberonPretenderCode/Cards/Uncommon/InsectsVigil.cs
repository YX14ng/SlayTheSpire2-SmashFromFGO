using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Vigilia del Insecto (虫之守望 / Insect's Vigil) — DESIGN-OBERON §6.3. 1⚡ Poder: al inicio de tu
/// turno, +10 Carga NP (up: además +5 Estrellas). Aplica <see cref="InsectsVigilPower"/> y le fija los
/// riders (el up enciende el +Estrellas).
/// </summary>
public sealed class InsectsVigil() : OberonCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Charge", 10), new DynamicVar("Stars", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<InsectsVigilPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.Charge = DynamicVars["Charge"].IntValue;
            power.Stars = DynamicVars["Stars"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(5m);
}
