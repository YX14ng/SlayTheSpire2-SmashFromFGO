using FGOCore.FGOCoreCode.DragonScales;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>
/// Muralla de Escamas (鳞甲壁垒 / Scale Bulwark) — el SEGUNDO sink ACTIVO de la Sangre de Dragón
/// (DESIGN-REVIEW-2: la SdD casi sólo subía; ScaleEruption la descarga a DAÑO, ésta la descarga a
/// BLOQUEO). 1⚡ Hab: 6 de Bloqueo base + 2 de Bloqueo por cada punto de SdD, hasta <see cref="ScaleCap"/>
/// puntos — pero NO consume la armadura (la piel sigue protegiendo). El otro lado de la moneda de la
/// erupción: cuando ya tenés escamas gruesas, ésta las traduce en un muro sin gastarlas. Patrón
/// HerosBackswing (escalado acotado por SdD) aplicado a Bloqueo en vez de daño.
/// </summary>
public sealed class ScaleBulwark() : SiegfriedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int ScaleCap = 6; // tope de puntos de SdD que aportan Bloqueo (techo §1.bis)

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Move), new DynamicVar("PerScale", 2), new DynamicVar("MaxScales", ScaleCap)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override bool ShouldGlowGoldInternal => GlowAtScales(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var counted = System.Math.Min(Scales, DynamicVars["MaxScales"].IntValue);
        var total = DynamicVars.Block.BaseValue + counted * DynamicVars["PerScale"].IntValue;
        await CreatureCmd.GainBlock(Owner.Creature, total, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
