using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Respiración de Combate (战息) — DESIGN-OKITA §5.2. 1⚡ Hab: 6 Bloqueo; +1 *Aliento (up: 9 / +1).
/// Recuperador de Aliento común.
/// </summary>
public sealed class BattleBreath() : OkitaCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Move), new DynamicVar("Breath", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await Aliento.Gain(Owner.Creature, DynamicVars["Breath"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m); // 6 -> 9
}
