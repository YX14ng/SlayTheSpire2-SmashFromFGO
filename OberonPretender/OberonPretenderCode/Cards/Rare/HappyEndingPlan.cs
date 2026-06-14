using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Np;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Plan del Final Feliz (幸福结局计划 / Happy Ending Plan) — DESIGN-OBERON §6.4. 2⚡ Poder: al jugarla
/// +2 de Bendición de Sobrecarga; al inicio de cada turno +1 (empuja los ultis a ≥150 → el sueño masivo).
/// Aplica <see cref="HappyEndingPlanPower"/> (el +1/turno) y <see cref="OverchargeBlessingPower"/> de
/// entrada. El up sube el bono de entrada.
/// </summary>
public sealed class HappyEndingPlan() : OberonCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Entry", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<OverchargeBlessingPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HappyEndingPlanPower>(Owner.Creature, 1m, Owner.Creature, this);
        await PowerCmd.Apply<OverchargeBlessingPower>(Owner.Creature, DynamicVars["Entry"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Entry"].UpgradeValueBy(1m);
}
