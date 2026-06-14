using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Paso Shukuchi (缩地步) — DESIGN-OKITA §5.2. 0⚡ Hab, RÁFAGA 1: robá 1; +10★ (up: robá 2).
/// El cantrip ráfaga. Paga el segundo coste vía <see cref="Rafaga.Pay"/>.
/// </summary>
public sealed class ShukuchiStep() : OkitaCard(0, CardType.Skill, CardRarity.Common, TargetType.Self), IRafagaCard
{
    public int RafagaCost => 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<AlientoPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => Rafaga.IsPlayable(Owner.Creature, RafagaCost);

    protected override bool ShouldGlowGoldInternal => Rafaga.ShouldGlow(Owner.Creature, RafagaCost);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Rafaga.Pay(choiceContext, Owner.Creature, RafagaCost, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m); // robá 1 -> 2
}
