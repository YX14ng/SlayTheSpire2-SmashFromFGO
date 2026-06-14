using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Desenfundar (拔刀, ESPEJO B) — DESIGN-OKITA §5.2. 0⚡ Hab: si tenés ≥50 Carga NP, perdé 50 NP →
/// +50★ (up: consume 30). El maná estalla en chispas. Glow cuando es pagable. Par espejo de
/// <see cref="Sheathe"/>.
/// </summary>
public sealed class Unsheathe() : OkitaCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int StarGain = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCost", 50), new DynamicVar("Stars", StarGain)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["NpCost"].IntValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cost = DynamicVars["NpCost"].IntValue;
        if (!await NpCharge.Spend(Owner.Creature, cost, this)) return;
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCost"].UpgradeValueBy(-20m); // 50 -> 30
}
