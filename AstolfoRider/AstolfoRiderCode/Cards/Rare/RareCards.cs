using AstolfoRider.AstolfoRiderCode.Caprice;
using AstolfoRider.AstolfoRiderCode.Cards.Uncommon;
using AstolfoRider.AstolfoRiderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Cards.Rare;

public sealed class EvaporatedReasonDPlus() : AstolfoCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<EvaporatedReasonDPlusPower>("Reason", 1m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<EvaporatedReasonDPlusPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class PerfectImprovisation() : AstolfoCard(
    1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if ((await UncommonRules.ChooseCaprice(c, this)).HasValue)
            await PowerCmd.Apply<PerfectImprovisationPower>(
                c, Owner.Creature, 1m, Owner.Creature, this, silent: true);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class ThreeCapricesOneAdventure() : AstolfoCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ThreeCapricesOneAdventurePower>("Caprices", 3m), new EnergyVar(1)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<ThreeCapricesOneAdventurePower>(c, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class OptimalPath() : AstolfoCard(
    1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Exhaust, CardKeyword.Retain] : [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var chosenType = await UncommonRules.ChooseCaprice(c, this);
        if (!chosenType.HasValue) return;
        var options = PileType.Draw.GetPile(Owner).Cards
            .Concat(PileType.Discard.GetPile(Owner).Cards)
            .Where(card => card is ICommandTyped typed && !typed.IsNoblePhantasm &&
                           typed.CommandType == chosenType.Value).ToList();
        if (options.Count == 0) return;
        var selected = await CardSelectCmd.FromChooseACardScreen(c, options, Owner, false);
        if (selected != null) await CardPileCmd.Add(selected, PileType.Hand);
    }
    protected override void OnUpgrade() { }
}

public sealed class MoonlessNight() : AstolfoCard(
    1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ArtifactPower>("Artifact", 1m), new DynamicVar("NpCharge", 0)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await UncommonRules.ChooseCaprice(c, this, refill: true);
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature, 1m, Owner.Creature, this);
        if (DynamicVars["NpCharge"].IntValue > 0)
            await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class LuckyHit() : AstolfoCard(
    0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 30), new DynamicVar("CritReady", 1)];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (await CritStars.Spend(c, Owner.Creature, DynamicVars["StarCost"].IntValue, this))
            await Criticals.GrantReady(c, Owner.Creature, 1, this);
    }
    protected override void OnUpgrade() => DynamicVars["StarCost"].UpgradeValueBy(-10m);
}

public sealed class StarsIDoNotNeed() : AstolfoCard(
    1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("MaxStars", 50), new DynamicVar("DamagePer10", 2), new DynamicVar("MaxDamage", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var spend = Math.Min(50, CritStars.Of(Owner.Creature)) / 10 * 10;
        if (spend <= 0 || !await CritStars.Spend(c, Owner.Creature, spend, this)) return;
        var damage = Math.Min(DynamicVars["MaxDamage"].IntValue,
            spend / 10 * DynamicVars["DamagePer10"].IntValue);
        await PowerCmd.Apply<NextNormalAttackBonusPower>(
            c, Owner.Creature, damage, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["DamagePer10"].UpgradeValueBy(1m);
        DynamicVars["MaxDamage"].UpgradeValueBy(5m);
    }
}

public sealed class GallopAtFullSpeed() : AstolfoCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<FullSpeedGallopPower>("DamagePerHit", 1m), new DynamicVar("MaxDamage", 6)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<FullSpeedGallopPower>(c, Owner.Creature,
            DynamicVars["DamagePerHit"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade()
    {
        DynamicVars["DamagePerHit"].UpgradeValueBy(1m);
        DynamicVars["MaxDamage"].UpgradeValueBy(4m);
    }
}

public sealed class FasterThanAnArrow() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, CommandType.Quick)
{
    private const int Hits = 5;
    public override int DamagePortions => Hits;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue, Hits))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitCount(Hits)
            .OnlyPlayAnimOnce().WithHitFx("vfx/vfx_starry_impact").Execute(c);
        await CritStars.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class LeapBetweenDimensions() : AstolfoCard(
    1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 50), new DynamicVar("Evasion", 2), new BlockVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (await CritStars.Spend(c, Owner.Creature, DynamicVars["StarCost"].IntValue, this))
            await Evasion.Grant(c, Owner.Creature, 2, this);
        else await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["StarCost"].UpgradeValueBy(-10m);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}

public sealed class WorldReverse() : AstolfoCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WorldReversePower>("Block", 8m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<WorldReversePower>(c, Owner.Creature,
            DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(4m);
}

public sealed class IWasNotThere() : AstolfoCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Evasion", 2), new EnergyVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await Evasion.Grant(c, Owner.Creature, 2, this);
        if (!IsUpgraded)
            await PowerCmd.Apply<EnergyPenaltyNextTurnPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() { }
}

public sealed class HippogriffUp() : AstolfoCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 50), new BlockVar(10m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var noEvasion = Evasion.Of(Owner.Creature) == 0;
        await NpCharge.Gain(c, Owner.Creature, 50, this);
        if (noEvasion) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class CrashFromTheImpossible() : AstolfoCommandCard(
    3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, CommandType.Buster)
{
    public override int DamagePortions => Math.Max(1, Owner?.Creature.CombatState?.HittableEnemies.Count ?? 1);
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(18m, ValueProp.Move), new DynamicVar("PerEvasion", 6), new DynamicVar("MaxBonus", 18)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var evasion = Math.Min(EvasionPower.MaxStacks, Evasion.Of(Owner.Creature));
        if (evasion > 0) await Evasion.Spend(c, Owner.Creature, evasion, this);
        var bonus = Math.Min(DynamicVars["MaxBonus"].IntValue,
            evasion * DynamicVars["PerEvasion"].IntValue);
        var targets = Math.Max(1, Owner.Creature.CombatState!.HittableEnemies.Count);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue + bonus, targets))
            .FromCardFgoCompatibility(this, p).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_heavy_blunt").SpawningHitVfxOnEachCreature().Execute(c);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["PerEvasion"].UpgradeValueBy(1m);
        DynamicVars["MaxBonus"].UpgradeValueBy(3m);
    }
}

public sealed class TrapOfArgalia() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, CommandType.Buster)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, ValueProp.Move), new DynamicVar("Artifact", 1), new PowerVar<ArgaliaKnockdownPower>("Knockdown", 1m), new PowerVar<WeakPower>("Weak", 0m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target)
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(c);
        if (p.Target.IsDead) return;
        if (p.Target.GetPower<ArtifactPower>() is { } artifact)
        {
            if (artifact.Amount <= 1) await PowerCmd.Remove(artifact);
            else await PowerCmd.ModifyAmount(c, artifact, -1m, Owner.Creature, this);
        }
        await PowerCmd.Apply<ArgaliaKnockdownPower>(c, p.Target, 1m, Owner.Creature, this);
        if (DynamicVars["Weak"].BaseValue > 0)
            await PowerCmd.Apply<WeakPower>(c, p.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Weak"].UpgradeValueBy(2m);
    }
}

public sealed class CasseurDeLogistille() : AstolfoCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ArtifactPower>("Artifact", 2m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var player = p.Target ?? Owner.Creature;
        await Cleanse.RemoveDebuffs(player);
        await PowerCmd.Apply<ArtifactPower>(c, player, 2m, Owner.Creature, this);
        var enemy = Owner.Creature.CombatState?.HittableEnemies
            .OrderByDescending(target => target.Powers.Count(Cleanse.IsOffensiveBuff)).FirstOrDefault();
        if (enemy != null) await Cleanse.RemoveOffensiveBuffs(enemy);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class BlackLunaPanicCall() : AstolfoCommandCard(
    3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, CommandType.Buster)
{
    public override int DamagePortions => Math.Max(1, Owner?.Creature.CombatState?.HittableEnemies.Count ?? 1);
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(15m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 2m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var targets = Owner.Creature.CombatState?.HittableEnemies.ToList() ?? [];
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue, Math.Max(1, targets.Count)))
            .FromCardFgoCompatibility(this, p).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_heavy_blunt").SpawningHitVfxOnEachCreature().Execute(c);
        foreach (var enemy in targets.Where(x => !x.IsDead))
            await PowerCmd.Apply<WeakPower>(c, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["Weak"].UpgradeValueBy(1m);
    }
}

public sealed class BorrowedAkhilleusKosmos() : AstolfoCard(
    3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(16m, ValueProp.Move), new PowerVar<BufferPower>("Buffer", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        foreach (var player in Owner.Creature.CombatState?.PlayerCreatures.Where(x => !x.IsDead).ToList() ?? [])
            await CreatureCmd.GainBlock(player, DynamicVars.Block.BaseValue, ValueProp.Move, p);
        await PowerCmd.Apply<BufferPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class GoodDeedsWithoutThinking() : AstolfoCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<GoodDeedsWithoutThinkingPower>("Block", 5m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<GoodDeedsWithoutThinkingPower>(c, Owner.Creature,
            DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}

public sealed class AdventureContinues() : AstolfoCard(
    3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<AdventureContinuesPower>("Adventure", 1m), new DynamicVar("NpCharge", 20), new DynamicVar("Stars", 20)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<AdventureContinuesPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
