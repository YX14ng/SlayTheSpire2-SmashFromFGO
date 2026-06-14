using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Instinto de Duelista (决斗者直觉) — DESIGN-OKITA §5.3. 1⚡ Hab: robá 2; si tenés ≥10★: +10 Carga NP
/// (up: robá 3). Robo + NP. Glow cuando ≥10★.
/// </summary>
public sealed class DuelistsInstinct() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int StarThreshold = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => CritStars.Of(Owner.Creature) >= StarThreshold;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hadStars = CritStars.Of(Owner.Creature) >= StarThreshold;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        if (hadStars) await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m); // robá 2 -> 3
}
