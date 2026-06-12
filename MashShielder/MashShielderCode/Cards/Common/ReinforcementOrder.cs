using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Orden de Refuerzo — rediseño v2: 1E Habilidad, +30 de Carga NP y roba 1 (up +20).
/// El 高速咏唱 de Mash. Antes Bloqueo+robo, triplicado funcional de
/// GuardStep/TrainingSimulation.
/// </summary>
public sealed class ReinforcementOrder() : MashShielderCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 30),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
