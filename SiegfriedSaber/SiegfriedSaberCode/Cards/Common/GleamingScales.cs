using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>
/// Escamas Refulgentes (流光鳞甲 / Gleaming Scales) — el SEGUNDO generador de Estrellas de Siegfried
/// (DESIGN-REVIEW-2: el subtema ★ era feast-or-famine porque sólo GraniStalk + Das Rheingold producían
/// ★; con un solo generador, los payoffs de ★ quedaban muertos en la mayoría de runs). 1⚡ Hab: 5 de
/// Bloqueo + 15 Estrellas de Crítico. La luz que rebota en la piel del dragón: junta munición de ★ a la
/// vez que tanquea, dándole un segundo eje al subtema para que PiercingGlare/WyrmslayerGaze tengan con
/// qué proc. Patrón GraniStalk (CritStars.Gain) + un cuerpo de Bloqueo. El up sube SÓLO las ★.
/// </summary>
public sealed class GleamingScales() : SiegfriedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, ValueProp.Move), new DynamicVar("Stars", 15)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
