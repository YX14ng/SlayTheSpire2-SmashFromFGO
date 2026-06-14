using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Ojos en la Punta (剑尖之眼) — DESIGN-OKITA §5.2. 1⚡ At: 8 daño; si tenés ≥10★: +10 Carga NP
/// (up: 11 / +10). Rider calibrado al +10 garantizado del Haori. Glow cuando ≥10★.
/// </summary>
public sealed class EyesOnTheTip() : OkitaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int StarThreshold = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => CritStars.Of(Owner.Creature) >= StarThreshold;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hadStars = CritStars.Of(Owner.Creature) >= StarThreshold;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (hadStars) await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m); // 8 -> 11
}
