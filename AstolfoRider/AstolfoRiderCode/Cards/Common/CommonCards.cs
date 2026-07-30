using AstolfoRider.AstolfoRiderCode.Caprice;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Cards.Common;

internal static class CommonRules
{
    public static bool Fulfilled(Creature owner) => Caprices.FulfilledThisTurn(owner) > 0;
    public static bool IsWeak(Creature target) => target.GetPowerAmount<WeakPower>() > 0;
}

public sealed class UnexpectedGallop() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, CommandType.Quick)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var fulfills = MatchesCaprice;
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue)).FromCardFgoCompatibility(this, p)
            .Targeting(p.Target).WithHitFx("vfx/vfx_starry_impact").Execute(c);
        if (fulfills) await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class LancePirouette() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, CommandType.Quick)
{
    private const int Hits = 2;
    public override int DamagePortions => Hits;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", Hits), new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var critical = Criticals.IsCritical(p);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue, Hits)).FromCardFgoCompatibility(this, p)
            .Targeting(p.Target).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        if (critical) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}

public sealed class UnbridledCharge() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, CommandType.Buster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move), new DynamicVar("OffCapriceBonus", 4)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var current = Caprices.Current(Owner.Creature);
        var bonus = current.HasValue && current != CommandType.Buster
            ? DynamicVars["OffCapriceBonus"].BaseValue : 0m;
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue + bonus))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target)
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(c);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["OffCapriceBonus"].UpgradeValueBy(1m);
    }
}

public sealed class ArgaliaPoint() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, CommandType.Arts)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 20), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var weak = CommonRules.IsWeak(p.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue)).FromCardFgoCompatibility(this, p)
            .Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (weak) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class CavalrySword() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, CommandType.Buster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("BlockPer20", 2), new DynamicVar("MaxBlock", 8)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(c);
        var block = Math.Min(DynamicVars["MaxBlock"].IntValue,
            CritStars.Of(Owner.Creature) / 20 * DynamicVars["BlockPer20"].IntValue);
        if (block > 0) await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Unpowered, p);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["MaxBlock"].UpgradeValueBy(2m);
    }
}

public sealed class HippogriffDive() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, CommandType.Quick)
{
    public override int DamagePortions => Math.Max(1, Owner?.Creature.CombatState?.HittableEnemies.Count ?? 1);
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("StarThreshold", 50), new DynamicVar("Bonus", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var damage = DynamicVars.Damage.BaseValue +
                     (CritStars.Of(Owner.Creature) >= 50 ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(WithCapriceDamage(damage, Math.Max(1, Owner.Creature.CombatState!.HittableEnemies.Count))).FromCardFgoCompatibility(this, p)
            .TargetingAllOpponents(Owner.Creature.CombatState!).WithHitFx("vfx/vfx_starry_impact")
            .SpawningHitVfxOnEachCreature().Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class GoodSpiritsLance() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, CommandType.Buster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(16m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var fulfilled = CommonRules.Fulfilled(Owner.Creature);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target)
            .WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        if (fulfilled) await NpCharge.Gain(c, Owner.Creature, 10, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5m);
}

public sealed class CuttingFeather() : AstolfoCommandCard(
    0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, CommandType.Quick)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue)).FromCardFgoCompatibility(this, p)
            .Targeting(p.Target).WithHitFx("vfx/vfx_starry_impact").Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class ChangeDirection() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await Caprices.Reroll(c, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class ForgotThePlan() : AstolfoCard(
    0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 10), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await Caprices.DiscardCurrent(c, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, 10, this);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class ImprovisedDefense() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, ValueProp.Move), new DynamicVar("Bonus", 10), new DynamicVar("BusterBlock", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var type = Caprices.Current(Owner.Creature);
        var block = DynamicVars.Block.BaseValue +
                    (type == CommandType.Buster ? DynamicVars["BusterBlock"].BaseValue : 0m);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, p);
        if (type == CommandType.Quick) await CritStars.Gain(c, Owner.Creature, 10, this);
        if (type == CommandType.Arts) await NpCharge.Gain(c, Owner.Creature, 10, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class AbruptManeuver() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Move), new DynamicVar("Stars", 20), new DynamicVar("Bonus", 6)];
    protected override bool ShouldGlowGoldInternal => CritStars.CanPay(Owner.Creature, 20);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var paid = await CritStars.Spend(c, Owner.Creature, 20, this);
        var block = DynamicVars.Block.BaseValue + (paid ? DynamicVars["Bonus"].BaseValue : 0m);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, p);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}

public sealed class WingsAsShield() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self), IRetainWhileEvading
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class SoftLanding() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (CommonRules.Fulfilled(Owner.Creature)) await NpCharge.Gain(c, Owner.Creature, 10, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class SharedSurprise() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Move), new DynamicVar("Bonus", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        var block = DynamicVars.Block.BaseValue +
                    (!CommonRules.Fulfilled(Owner.Creature) ? DynamicVars["Bonus"].BaseValue : 0m);
        await CreatureCmd.GainBlock(target, block, ValueProp.Move, p);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}

public sealed class StarsOnTheRoad() : AstolfoCard(
    0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCost", 50), new DynamicVar("Stars", 50)];
    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["NpCost"].IntValue;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (await NpCharge.Spend(c, Owner.Creature, DynamicVars["NpCost"].IntValue, this))
            await CritStars.Gain(c, Owner.Creature, 50, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCost"].UpgradeValueBy(-10m);
}

public sealed class ShortcutToTheSky() : AstolfoCard(
    0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 50), new DynamicVar("NpCharge", 50)];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (await CritStars.Spend(c, Owner.Creature, DynamicVars["StarCost"].IntValue, this))
            await NpCharge.Gain(c, Owner.Creature, 50, this);
    }
    protected override void OnUpgrade() => DynamicVars["StarCost"].UpgradeValueBy(-10m);
}

public sealed class NothingHappened() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Move), new DynamicVar("Debuffs", 1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (CommonRules.Fulfilled(Owner.Creature)) await Cleanse.RemoveDebuffs(Owner.Creature, 1);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class TrifasStep() : AstolfoCard(
    0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new DynamicVar("Discard", 1), new DynamicVar("Stars", 10), new DynamicVar("NpCharge", 0)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, 1, Owner);
        var selected = (await CardSelectCmd.FromHandForDiscard(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this)).FirstOrDefault();
        if (selected == null) return;
        var matches = selected is ICommandTyped typed && !typed.IsNoblePhantasm &&
                      Caprices.Matches(Owner.Creature, typed.CommandType);
        await CardCmd.Discard(c, [selected]);
        if (!matches) return;
        await CritStars.Gain(c, Owner.Creature, 10, this);
        if (DynamicVars["NpCharge"].IntValue > 0)
            await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class HornCall() : AstolfoCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>("Weak", 1m), new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        foreach (var enemy in Owner.Creature.CombatState?.HittableEnemies.ToList() ?? [])
            await PowerCmd.Apply<WeakPower>(c, enemy, 1m, Owner.Creature, this);
        if (CommonRules.Fulfilled(Owner.Creature))
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
