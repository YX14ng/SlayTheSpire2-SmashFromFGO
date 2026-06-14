using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Interés a Mi Favor (利息归我 / Interest in My Favor) — DESIGN-OBERON §6.3. 2⚡ Poder: cuando jugás
/// una carta de Préstamo, +10 Carga NP (up: además +10 Estrellas). Aplica <see cref="InterestInMyFavorPower"/>
/// y le fija los riders desde los DynamicVars (el up enciende el +Estrellas).
/// </summary>
public sealed class InterestInMyFavor() : OberonCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Charge", 10), new DynamicVar("Stars", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<InterestInMyFavorPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.Charge = DynamicVars["Charge"].IntValue;
            power.Stars = DynamicVars["Stars"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
