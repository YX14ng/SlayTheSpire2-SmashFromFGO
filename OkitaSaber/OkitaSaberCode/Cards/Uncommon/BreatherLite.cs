using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Respiro (小憩, Breather lite) — DESIGN-OKITA §5.3. 0⚡ Hab: 3 Bloqueo; +1 *Aliento (up: 5 / +2).
/// Recuperador 0⚡.
/// </summary>
public sealed class BreatherLite() : OkitaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(3m, ValueProp.Move), new DynamicVar("Breath", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await Aliento.Gain(Owner.Creature, DynamicVars["Breath"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);     // 3 -> 5
        DynamicVars["Breath"].UpgradeValueBy(1m); // +1 -> +2
    }
}
