using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Esgrima Defensiva (守势剑术) — DESIGN-OKITA §5.3. 1⚡ Hab: 9 Bloqueo; +10★ (up: 12 / +20★).
/// Defensa → ★.
/// </summary>
public sealed class DefensiveFencing() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(9m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);    // 9 -> 12
        DynamicVars["Stars"].UpgradeValueBy(10m); // +10 -> +20
    }
}
