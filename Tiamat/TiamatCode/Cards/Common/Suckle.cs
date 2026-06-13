using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Tiamat.TiamatCode.Cards.Common;

/// <summary>Amamantar — ALIMENTAR: +2 Crianza (escala a todo el enjambre). Si tenés 3+ Laḫmu, robá 1.</summary>
public sealed class Suckle() : TiamatCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Nurture", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<LahmuSwarmPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Lahmu.Feed(Owner.Creature, DynamicVars["Nurture"].IntValue, this);
        if (Lahmu.Count(Owner.Creature) >= 3 && Owner.Creature.Player != null)
        {
            await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Creature.Player);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Nurture"].UpgradeValueBy(1m);
}
