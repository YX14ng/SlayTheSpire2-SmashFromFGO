using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>Guardia de Escamas — feeder defensivo del motor SdD (bloqueo + escamas, sin NP). SKILL §2:
/// bloqueo 1⚡ con rider = 4-7; 6 + rider +1 SdD persistente. El up sube el BLOQUEO (+3); el SdD es
/// FIJO +1 (anti-apilado P3: no rompe el umbral SdD≥3 ni el techo del pierce de la Hoja de Tilo).</summary>
public sealed class DragonScaleGuard() : SiegfriedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move), new DynamicVar("Scales", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
