using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Estandarte de la Rebelión (叛逆旗帜) — DESIGN-MORDRED §5.2. 2⚡ Poder: cada cambio de forma +10
/// Estrellas y +5 de Carga NP (up: y robá 1). El motor de FORMAS hecho payoff recurrente. Aplica
/// <see cref="BannerOfRebellionPower"/> (IFormChangeListener); los valores y el robo son campos
/// settables que la carta fija desde sus DynamicVars. Patrón GoldenRule (poder con extra gateado por up).
/// </summary>
public sealed class BannerOfRebellion() : MordredCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 10), new DynamicVar("NpCharge", 5), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<BannerOfRebellionPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.StarsPerSwitch = DynamicVars["Stars"].IntValue;
            power.NpPerSwitch = DynamicVars["NpCharge"].IntValue;
            power.DrawsPerSwitch = IsUpgraded ? DynamicVars.Cards.IntValue : 0;
        }
    }
}
