using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Stars;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Pétalos de Nomeolvides (勿忘草花瓣 / Forget-me-not Petals) — DESIGN-OBERON §6.2. 1⚡ Habilidad:
/// 4 de Bloqueo; +10 Estrellas (up 6 / +20). El defensivo que alimenta la banca de estrellas (las
/// flores del prado del sueño). El up sube ambos.
/// </summary>
public sealed class ForgetMeNotPetals() : OberonCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(4m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
