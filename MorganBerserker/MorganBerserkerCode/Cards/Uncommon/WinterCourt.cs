using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Corte del Invierno (冬之宫廷) — rediseño v2: 2⚡ Poder. Al inicio de tu turno:
/// añade 1 Arma del Caballero a tu mano. Cuando jugás un Arma del Caballero:
/// +10 Estrellas de Crítico (generador sostenido + payoff de la tribu en una carta).
/// Mejora: −1⚡ y además roba 1 al inicio del turno (parche P10: devuelve el robo
/// sostenido que el rediseño le quitó al pool).
/// </summary>
public sealed class WinterCourt() : MorganCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WinterCourtPower>("Stacks", 1m),
        new DynamicVar("Stars", WinterCourtPower.StarsPerArm)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WinterCourtPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<WinterCourtDrawPower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
