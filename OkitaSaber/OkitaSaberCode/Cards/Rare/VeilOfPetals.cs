using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Velo de Pétalos (花瓣之帷) — DESIGN-OKITA §5.4. 2⚡ Hab, Exhaust: 20 Bloqueo; +20★ (up: 26 Bloqueo).
/// Botón de pánico de una frágil.
/// </summary>
public sealed class VeilOfPetals() : OkitaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(20m, ValueProp.Move), new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(6m); // 20 -> 26
}
