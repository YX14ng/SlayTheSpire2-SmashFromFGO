using FGOCore.FGOCoreCode.DragonScales;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>
/// Guardia de Piel de Dragón (龙皮守势 / Wyrmskin Guard) — feeder defensivo de profundidad
/// (DESIGN-REVIEW-2: el pool de 24 necesitaba más opciones sobre el motor SdD). 1⚡ Hab: 5 de Bloqueo +
/// 2 de Sangre de Dragón. El espejo "más escamas, menos muro" de Guardia de Escamas (que da 6 Bloqueo /
/// 1 SdD): siembra la armadura más rápido para alimentar a Balmung, ScaleBulwark y los Cazadragones.
/// Patrón DragonScaleGuard. El up sube SÓLO la SdD (el plan de motor, no el bloqueo puntual).
/// </summary>
public sealed class WyrmskinGuard() : SiegfriedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, ValueProp.Move), new DynamicVar("Scales", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<DragonScalesPower>(choiceContext, Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Scales"].UpgradeValueBy(1m);
}
