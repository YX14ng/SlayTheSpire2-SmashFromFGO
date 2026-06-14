using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Envainar (收刀, ESPEJO A) — DESIGN-OKITA §5.2. 0⚡ Hab: si tenés ≥50★, perdé 50★ → +50 Carga NP
/// (up: consume 30). La luz vuelve a la vaina. Glow cuando es pagable. Par espejo de
/// <see cref="Unsheathe"/>.
/// </summary>
public sealed class Sheathe() : OkitaCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int NpGain = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 50), new DynamicVar("NpCharge", NpGain)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cost = DynamicVars["StarCost"].IntValue;
        if (!CritStars.CanPay(Owner.Creature, cost)) return;
        await CritStars.Gain(Owner.Creature, -cost, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["StarCost"].UpgradeValueBy(-20m); // 50 -> 30
}
