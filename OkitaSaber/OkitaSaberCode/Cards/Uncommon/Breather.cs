using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Alto para Respirar (喘息) — DESIGN-OKITA §5.3. 2⚡ Hab: 14 Bloqueo; +2 *Aliento (up: 18 / +4).
/// Block premium + Aliento.
/// </summary>
public sealed class Breather() : OkitaCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(14m, ValueProp.Move), new DynamicVar("Breath", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await Aliento.Gain(Owner.Creature, DynamicVars["Breath"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);     // 14 -> 18
        DynamicVars["Breath"].UpgradeValueBy(2m); // +2 -> +4
    }
}
