using KagetoraLancer.KagetoraLancerCode.Cards.Special;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Rare;

public sealed class WhiteFlameA() : KagetoraCard(2, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WhiteFlamePower>("NpCharge", 10m), new DynamicVar("Stars", 10)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<WhiteFlamePower>(c, Owner.Creature, 10m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class JeweledPagodaC() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>("Strength", 1m), new DynamicVar("NpCharge", 20), new DynamicVar("Overcharge", 2)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        await PowerCmd.Apply<StrengthPower>(c, target, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        if (target.HasPower<CommandBonusPower>() || target.HasPower<NpChargePower>())
            await PowerCmd.Apply<OverchargePreparationPower>(c, target, 1m, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars["Strength"].UpgradeValueBy(1m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class EightFormationsOfBishamonten() : KagetoraCard(2, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<EightFormationsPower>("EightFormations", 1m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<EightFormationsPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class WisdomOfEightyFourThousandTeachings() : KagetoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4), new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        await NpCharge.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class VowOfBishamonten() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 50), new PowerVar<ArtifactPower>("Artifact", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await NpCharge.Gain(c, Owner.Creature, 50, this);
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["Artifact"].UpgradeValueBy(1m);
}

public sealed class WhiteFlameColdAndBurning() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("NpCharge", 20), new PowerVar<VulnerablePower>("Vulnerable", 2m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (!p.Target.IsDead) await PowerCmd.Apply<VulnerablePower>(c, p.Target, 2m, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class TwoRulerEvasions() : KagetoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BufferPower>("Buffer", 2m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<BufferPower>(c, Owner.Creature, DynamicVars["Buffer"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Buffer"].UpgradeValueBy(1m);
}

public sealed class TreasureIsInTheHeart() : KagetoraCard(2, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TreasureInHeartPower>("NpCharge", 10m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<TreasureInHeartPower>(c, Owner.Creature, DynamicVars["NpCharge"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class SendSaltToTheEnemy() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(6m), new BlockVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        await CreatureCmd.Heal(target, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(target, DynamicVars.Block.BaseValue, ValueProp.Move, p);
    }
    protected override void OnUpgrade() { DynamicVars.Heal.UpgradeValueBy(3m); DynamicVars.Block.UpgradeValueBy(4m); }
}

public sealed class WallsOfKasugayama() : KagetoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(20m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(6m);
}

public sealed class FieldJudge() : KagetoraCard(2, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FieldJudgePower>("Block", 8m), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<FieldJudgePower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() { DynamicVars["Block"].UpgradeValueBy(4m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class SipAtTheCenterOfTheArmy() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>("Intangible", 1m), new DynamicVar("Stars", 20), new CardsVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<IntangiblePower>(c, Owner.Creature, 1m, Owner.Creature, this);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade() { DynamicVars["Stars"].UpgradeValueBy(10m); DynamicVars.Cards.UpgradeValueBy(1m); }
}

public sealed class BitenWheelFormation() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2m, ValueProp.Move), new DynamicVar("Hits", 8), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 8 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await CritStars.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class ShiranuiBlade() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var kenshin = Owner.Creature.HasPower<KenshinFormPower>();
        if (kenshin && p.Target.Block > 0)
            await CreatureCmdCompatibility.LoseBlock(c, p.Target, p.Target.Block, Owner.Creature);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        if (!kenshin) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(6m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class FullHoushoutsukigeGallop() : KagetoraCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move), new DynamicVar("Hits", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        for (var hit = 0; hit < 3; hit++)
        foreach (var target in Owner.Creature.CombatState!.HittableEnemies.ToList())
            if (!target.IsDead) await CreatureCmdCompatibility.Damage(c, target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class Kawanakajima() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20m, ValueProp.Move), new DynamicVar("Bonus", 8)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var major = Owner.Creature.CombatState?.Encounter?.RoomType is RoomType.Elite or RoomType.Boss;
        var damage = DynamicVars.Damage.BaseValue + (major ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(6m); DynamicVars["Bonus"].UpgradeValueBy(2m); }
}

public sealed class EightWeaponsUnleashed() : KagetoraCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", 4), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 4 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await CritStars.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class VictoryIsInTheFeet() : KagetoraCard(2, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Feet)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VictoryIsInTheFeetPower>("Victory", 1m), new DynamicVar("Stars", 20), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<VictoryIsInTheFeetPower>(c, Owner.Creature, IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade() { }
}

public sealed class FortuneArmourAndMeritA() : KagetoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BufferPower>("Buffer", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var state = Owner.Creature.CombatState;
        var doctrine = Owner.Creature.GetPower<DoctrinePower>();
        if (state != null && doctrine != null)
        {
            var options = new List<CardModel>();
            if ((doctrine.ProgressMask & 1) == 0) options.Add(state.CreateCard(ModelDb.Card<ChooseHeaven>(), Owner));
            if ((doctrine.ProgressMask & 2) == 0) options.Add(state.CreateCard(ModelDb.Card<ChooseChest>(), Owner));
            if ((doctrine.ProgressMask & 4) == 0) options.Add(state.CreateCard(ModelDb.Card<ChooseFeet>(), Owner));
            var selected = await CardSelectCmd.FromChooseACardScreen(c, options, Owner, false);
            if (selected is IPreceptChoice choice)
            {
                Precept = choice.ChosenPrecept;
                await PowerCmd.Apply<ForcedDoctrineAdvancePower>(c, Owner.Creature, 1m, Owner.Creature, this, silent: true);
                Owner.Creature.GetPower<ForcedDoctrineAdvancePower>()?.Arm(this);
            }
        }
        await PowerCmd.Apply<BufferPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class ManifestationOfBishamonten() : KagetoraCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BishamontenManifestationPower>("Cycles", 3m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<BishamontenManifestationPower>(c, Owner.Creature, 3m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
