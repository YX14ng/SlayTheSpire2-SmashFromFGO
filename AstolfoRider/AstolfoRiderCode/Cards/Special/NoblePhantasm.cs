using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Cards.Special;

public sealed class Hippogriff() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Event, TargetType.AllEnemies, CommandType.Quick)
{
    public const int ChargeCost = 100;
    public override bool IsNoblePhantasm => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    protected override bool ShouldGlowGoldInternal => IsPlayable;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Move),
        new DynamicVar("Evasion", 3),
        new DynamicVar("OverchargeStars", 10)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<EvasionPower>(),
        HoverTipFactory.FromPower<SureHitPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var tier = Math.Clamp(
            await NpCharge.ConsumeAllForNpCard(context, Owner.Creature, ChargeCost, this), 100, 500);
        await SureHit.Grant(context, Owner.Creature, 1, this);
        await DamageCmd.Attack(NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_starry_impact").SpawningHitVfxOnEachCreature().Execute(context);

        await Evasion.Grant(context, Owner.Creature,
            Math.Max(0, EvasionPower.MaxStacks - Evasion.Of(Owner.Creature)), this);
        var oc = Math.Clamp(tier / 100, 1, 5);
        var extraStars = 5 + 5 * oc;
        await CritStars.Gain(context, Owner.Creature, extraStars, this);
    }
}
