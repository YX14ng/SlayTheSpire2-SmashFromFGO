using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ShutenDouji.ShutenDoujiCode.Powers;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Cards.Rare;

public sealed class ShinpenKidoku() : ShutenCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<AccelerantPower>("Accelerant", 1m)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<AccelerantPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<AccelerantPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class ThousandPurplesTenThousandReds() : ShutenCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, ShutenStyle.Assassin)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 10m), new PowerVar<WeakPower>("Weak", 2m), new PowerVar<VulnerablePower>("Vulnerable", 2m), new DynamicVar("Sake", 30)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<PoisonPower>(context, enemy, DynamicVars["Poison"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<WeakPower>(context, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(context, enemy, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(4m);
}

public sealed class MountOoeBanquet() : ShutenCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Sake", 20), new PowerVar<MountOoeBanquetPower>("Poison", 2m)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<MountOoeBanquetPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<MountOoeBanquetPower>(context, Owner.Creature,
                DynamicVars["Poison"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(1m);
}

public sealed class FruityAromaEx() : ShutenCard(
    1, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Sake", 20), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<FruityAromaExPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<FruityAromaExPower>(context, Owner.Creature,
                IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class SeveredHead() : ShutenCard(
    2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(22m, ValueProp.Move), new DynamicVar("NpCharge", 20), new PowerVar<GutsPower>("Guts", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_bloody_impact").Execute(context);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await PowerCmd.Apply<GutsPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}

public sealed class PoisonedBanquet() : ShutenCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Sake", 30)];
    protected override bool IsPlayable => CombatState?.HittableEnemies.Any(e => e.HasPower<PoisonPower>()) == true;
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (cardPlay.Target.GetPower<PoisonPower>() is not { } poison) return;
        await CreatureCmd.Damage(context, cardPlay.Target, poison.Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        if (!cardPlay.Target.IsDead && cardPlay.Target.GetPower<PoisonPower>() is { } remaining)
            await PowerCmd.Decrement(remaining);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class VoiceThatMeltsReason() : ShutenCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, ShutenStyle.Assassin)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>("Weak", 2m), new DynamicVar("SkillSeal", 1), new DynamicVar("Sake", 30)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<SkillSealPower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(context, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
            await SkillSeal.Apply(context, enemy, DynamicVars["SkillSeal"].IntValue, Owner.Creature, this);
        }
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

public sealed class OrochiBlood() : ShutenCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 10), new DynamicVar("Stars", 10)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<OrochiBloodPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<OrochiBloodPower>(context, Owner.Creature,
                IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class HeadDance() : ShutenCard(
    2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    private const int Hits = 7;
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("Stars", 20)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_starry_impact").Execute(context);
        await CritStars.Gain(context, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}

public sealed class LastService() : ShutenCard(
    0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", 40)];
    protected override bool ShouldGlowGoldInternal => SakeBank.Current(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var spent = SakeBank.Current(Owner.Creature);
        if (spent <= 0 || !await SakeBank.Spend(context, Owner.Creature, spent, this)) return;
        var poison = spent * DynamicVars["Percent"].IntValue / 100;
        if (poison > 0) await PowerCmd.Apply<PoisonPower>(context, cardPlay.Target, poison, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Percent"].UpgradeValueBy(10m);
}

public sealed class ExtremeSpeedBeating() : ShutenCard(
    2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    private const int Hits = 6;
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("MaxSake", 50)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var spent = await SakeBank.SpendUpTo(context, Owner.Creature, DynamicVars["MaxSake"].IntValue, this);
        var damage = DynamicVars.Damage.BaseValue + spent / 10;
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(context);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class HakuWhiteFamiliar() : ShutenCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Unpowered), new DynamicVar("SakeCost", 10), new DynamicVar("Bonus", 4), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<HakuWhiteFamiliarPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<HakuWhiteFamiliarPower>(context, Owner.Creature,
                DynamicVars.Block.BaseValue, Owner.Creature, this);
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}

public sealed class GohoOni() : ShutenCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Sake", 10), new BlockVar(0m, ValueProp.Unpowered)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<GohoOniPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<GohoOniPower>(context, Owner.Creature,
                IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade()
    {
        DynamicVars["Sake"].UpgradeValueBy(10m);
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}

public sealed class NineHeadsOfTheDragon() : ShutenCard(
    3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, ShutenStyle.Caster), ICommandTyped
{
    private const int Hits = 3;
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("SakeCost", 50), new DynamicVar("Bonus", 2)];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var damage = DynamicVars.Damage.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState!).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(context);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class DistillThePoison() : ShutenCard(
    1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, ShutenStyle.Caster)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("MaxPoison", 10), new DynamicVar("SakePer", 3), new DynamicVar("NpPer", 3)];
    protected override bool IsPlayable => CombatState?.HittableEnemies.Any(e => e.HasPower<PoisonPower>()) == true;
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (cardPlay.Target.GetPower<PoisonPower>() is not { } poison) return;
        var removed = Math.Min(poison.Amount, DynamicVars["MaxPoison"].IntValue);
        if (poison.Amount <= removed) await PowerCmd.Remove(poison);
        else await PowerCmd.ModifyAmount(context, poison, -removed, Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, removed * DynamicVars["SakePer"].IntValue, this);
        await NpCharge.Gain(context, Owner.Creature, removed * DynamicVars["NpPer"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["MaxPoison"].UpgradeValueBy(5m);
}

public sealed class MagicalGirlAtFullPower() : ShutenCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("SakeCost", 20), new DynamicVar("PerHit", 5), new DynamicVar("Cap", 20)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<FullPowerMagicalGirlPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<FullPowerMagicalGirlPower>(context, Owner.Creature,
                IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade()
    {
        DynamicVars["PerHit"].UpgradeValueBy(1m);
        DynamicVars["Cap"].UpgradeValueBy(4m);
    }
}

public sealed class UnleashedGohoClub() : ShutenCard(
    2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(25m, ValueProp.Move), new DynamicVar("DebuffBonus", 10), new DynamicVar("KillSake", 50)];
    protected override bool ShouldGlowGoldInternal => CombatState?.HittableEnemies.Any(Uncommon.UncommonRules.HasDebuff) == true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var alive = cardPlay.Target.IsAlive;
        var damage = DynamicVars.Damage.BaseValue +
                     (Uncommon.UncommonRules.HasDebuff(cardPlay.Target) ? DynamicVars["DebuffBonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_heavy_blunt").Execute(context);
        if (alive && cardPlay.Target.IsDead)
            await SakeBank.Gain(context, Owner.Creature, DynamicVars["KillSake"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
        DynamicVars["DebuffBonus"].UpgradeValueBy(2m);
    }
}

public sealed class OniSalvation() : ShutenCard(
    2, CardType.Skill, CardRarity.Rare, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(12m, ValueProp.Move), new DynamicVar("Debuffs", 1), new DynamicVar("SakeCost", 30), new PowerVar<BufferPower>("Buffer", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        foreach (var player in Owner.Creature.CombatState!.PlayerCreatures)
        {
            await CreatureCmd.GainBlock(player, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
            await Cleanse.RemoveDebuffs(player, DynamicVars["Debuffs"].IntValue);
        }
        if (await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this))
            await PowerCmd.Apply<BufferPower>(context, Owner.Creature,
                DynamicVars["Buffer"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class OneSaintGraphTwoOutfits() : ShutenCard(
    2, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<OneSaintGraphTwoOutfitsPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<OneSaintGraphTwoOutfitsPower>(context, Owner.Creature,
                IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class BanquetNeverEnds() : ShutenCard(
    3, CardType.Power, CardRarity.Rare, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("SakeCost", 20), new EnergyVar(1)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<BanquetNeverEndsPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<BanquetNeverEndsPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
