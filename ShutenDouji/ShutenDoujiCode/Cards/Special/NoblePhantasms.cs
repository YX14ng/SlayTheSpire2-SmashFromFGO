using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Cards.Special;

public interface IShutenNpCard : ICommandTyped;

public abstract class ShutenNpCard(TargetType target, ShutenStyle style, CommandType commandType) :
    ShutenCard(0, CardType.Attack, CardRarity.Event, target, style), IShutenNpCard
{
    public const int ChargeCost = 100;
    public override bool IsShutenNp => true;
    CommandType ICommandTyped.CommandType => commandType;
    public bool IsNoblePhantasm => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected async Task<int> ConsumeTier(PlayerChoiceContext context) =>
        Math.Clamp(await NpCharge.ConsumeAllForNpCard(context, Owner.Creature, ChargeCost, this), 100, 500);

    protected Task ExhaustSibling(PlayerChoiceContext context) =>
        MainFile.ExhaustSiblingNp(context, Owner.Creature, this);
}

public sealed class SenjiBankoShinpenKidoku() : ShutenNpCard(
    TargetType.AllEnemies, ShutenStyle.Assassin, CommandType.Arts)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(28m, ValueProp.Move),
        new PowerVar<PoisonPower>("Poison", 8m),
        new PowerVar<WeakPower>("Weak", 1m),
        new PowerVar<VulnerablePower>("Vulnerable", 1m),
        new DynamicVar("Seal", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<SkillSealPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var tier = await ConsumeTier(context);
        var enemies = Owner.Creature.CombatState?.HittableEnemies.ToList() ?? [];

        foreach (var enemy in enemies)
        {
            if (enemy.GetPower<ArtifactPower>() is not { } artifact) continue;
            if (artifact.Amount <= 1) await PowerCmd.Remove(artifact);
            else await PowerCmd.ModifyAmount(context, artifact, -1m, Owner.Creature, this);
        }

        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_bloody_impact").SpawningHitVfxOnEachCreature().Execute(context);

        var oc = Math.Clamp(tier / 100, 1, 5);
        var poison = DynamicVars["Poison"].IntValue + (oc - 1) * 3;
        foreach (var enemy in enemies.Where(enemy => !enemy.IsDead))
        {
            await PowerCmd.Apply<WeakPower>(context, enemy, 1m, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(context, enemy, 1m, Owner.Creature, this);
            await SkillSeal.Apply(context, enemy, 1, Owner.Creature, this);
            await PowerCmd.Apply<PoisonPower>(context, enemy, poison, Owner.Creature, this);
        }

        await ExhaustSibling(context);
    }
}

public sealed class GohoShojoKuzuryuOsatsu() : ShutenNpCard(
    TargetType.AnyEnemy, ShutenStyle.Caster, CommandType.Buster)
{
    private const int Hits = 6;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Hits", Hits),
        new PowerVar<PoisonPower>("Poison", 5m),
        new DynamicVar("MaxSake", 50)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<SakePower>(),
        HoverTipFactory.FromPower<SureHitPower>(),
        HoverTipFactory.FromPower<PoisonPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;
        var tier = await ConsumeTier(context);
        var oc = Math.Clamp(tier / 100, 1, 5);
        var spent = await SakeBank.SpendUpTo(context, Owner.Creature,
            DynamicVars["MaxSake"].IntValue, this);
        var perHit = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue)
                     + (oc - 1) + spent / 10;

        await SureHit.Grant(context, Owner.Creature, 1, this);
        await DamageCmd.Attack(perHit).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(target).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(context);

        if (!target.IsDead)
        {
            var poison = DynamicVars["Poison"].IntValue + (oc - 1) * 2;
            await PowerCmd.Apply<PoisonPower>(context, target, poison, Owner.Creature, this);
        }

        await ExhaustSibling(context);
    }
}
