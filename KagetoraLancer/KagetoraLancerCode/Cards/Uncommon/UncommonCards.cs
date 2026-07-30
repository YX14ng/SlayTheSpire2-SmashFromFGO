using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Uncommon;

public sealed class WheelStrategy() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        if (Owner.Creature.GetPower<DoctrinePower>()?.AdvancedMaskThisTurn != 0)
            await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class FourHeavenlyStrikes() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class PrepareTheCavalry() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        var chosen = (await CardSelectCmd.FromHand(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), card => card != this, this)).FirstOrDefault();
        chosen?.GiveSingleTurnRetain();
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class MagicalCharge() : KagetoraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 30)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(20m);
}

public sealed class FormationRelay() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var doctrine = Owner.Creature.GetPower<DoctrinePower>();
        if (doctrine == null) return;
        var options = PileType.Discard.GetPile(Owner).Cards.Where(card =>
            card is IPreceptCard tagged && doctrine.WouldAdvanceAfter(Precept.Heaven, tagged.Precept)).ToList();
        if (options.Count == 0) return;
        var chosen = await CardSelectCmd.FromChooseACardScreen(c, options, Owner, false);
        if (chosen != null) await CardPileCmd.Add(chosen, PileType.Hand);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class CommandersGaze() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>("Weak", 2m), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await PowerCmd.Apply<WeakPower>(c, p.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

public sealed class VanguardMandate() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target?.Player ?? Owner;
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, target);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class HeavensFocus() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CritReady", 1), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await Criticals.GrantReady(c, Owner.Creature, DynamicVars["CritReady"].IntValue, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class ArmourInTheChestA() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>("Intangible", 1m), new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<IntangiblePower>(c, Owner.Creature, 1m, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class BulletCurtain() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(9m, ValueProp.Move), new PowerVar<BulletCurtainPower>("Stars", 20m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<BulletCurtainPower>(c, Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class RulersDefense() : KagetoraCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(14m, ValueProp.Move), new PowerVar<ArtifactPower>("Artifact", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class SereneCounterattack() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move), new PowerVar<SereneCounterPower>("Counter", 6m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<SereneCounterPower>(c, Owner.Creature, DynamicVars["Counter"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(2m); DynamicVars["Counter"].UpgradeValueBy(3m); }
}

public sealed class FearlessChest() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FearlessChestPower>("Block", 2m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<FearlessChestPower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(1m);
}

public sealed class TreasureInTheHeartB() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ArtifactPower>("Artifact", 2m), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["Artifact"].UpgradeValueBy(1m);
}

public sealed class SharedGuard() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move), new DynamicVar("AllyBlock", 4)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        foreach (var creature in Owner.Creature.CombatState!.PlayerCreatures)
        {
            var block = creature == Owner.Creature ? DynamicVars.Block.BaseValue : DynamicVars["AllyBlock"].BaseValue;
            await CreatureCmd.GainBlock(creature, block, ValueProp.Move, p);
        }
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["AllyBlock"].UpgradeValueBy(2m); }
}

public sealed class WallOfBanners() : KagetoraCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(14m, ValueProp.Move), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if ((Owner.Creature.GetPower<DoctrinePower>()?.AdvancedMaskThisTurn & ~(int)Precept.Chest) != 0)
            await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(4m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class JustPath() : KagetoraCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<JustPathPower>("Block", 6m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<JustPathPower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}

public sealed class MeritIsInTheFeetA() : KagetoraCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Feet)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>("Strength", 2m), new DynamicVar("AllyStrength", 1), new DynamicVar("Stars", 30)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        foreach (var creature in Owner.Creature.CombatState!.PlayerCreatures)
            await PowerCmd.Apply<StrengthPower>(c, creature, creature == Owner.Creature ? DynamicVars["Strength"].BaseValue : 1m, Owner.Creature, this);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars["Strength"].UpgradeValueBy(1m); DynamicVars["Stars"].UpgradeValueBy(20m); }
}

public sealed class HoushoutsukigeGallop() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(1m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class EightWeaponsOneWarrior() : KagetoraCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move), new DynamicVar("Hits", 2), new DynamicVar("StarsPer", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 2 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        var mask = Owner.Creature.GetPower<DoctrinePower>()?.AdvancedMaskThisTurn ?? 0;
        var count = ((mask & 1) != 0 ? 1 : 0) + ((mask & 2) != 0 ? 1 : 0) + ((mask & 4) != 0 ? 1 : 0);
        if (count > 0) await CritStars.Gain(c, Owner.Creature, count * 10, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class SpinningNaginata() : KagetoraCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).TargetingAllOpponents(Owner.Creature.CombatState!).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class RelentlessPursuit() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11m, ValueProp.Move), new DynamicVar("Bonus", 5)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var hasBuff = p.Target.GetPowerInstances<PowerModel>().Any(power => power.Type == PowerType.Buff);
        var damage = DynamicVars.Damage.BaseValue + (hasBuff ? 0m : DynamicVars["Bonus"].BaseValue);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["Bonus"].UpgradeValueBy(2m); }
}

public sealed class AlternatingAssault() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        if (Criticals.IsCritical(p)) await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(1m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class RetreatIsHell() : KagetoraCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move), new DynamicVar("Stars", 30)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var alive = p.Target.IsAlive;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        if (alive && p.Target.IsDead) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(5m); DynamicVars["Stars"].UpgradeValueBy(20m); }
}

public sealed class RidingC() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, Precept.Feet)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RidingPower>("Stars", 10m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<RidingPower>(c, Owner.Creature, 10m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class ArmyFootsteps() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Feet)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        var card = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault(x => x is IPreceptCard { Precept: Precept.Feet });
        if (card != null) await CardPileCmd.Add(card, PileType.Hand);
    }
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class GeneralsDoctrine() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GeneralsDoctrinePower>("Block", 3m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<GeneralsDoctrinePower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}

public sealed class DivinityCToA() : KagetoraCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DivinityPower>("Damage", 3m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<DivinityPower>(c, Owner.Creature, 3m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
