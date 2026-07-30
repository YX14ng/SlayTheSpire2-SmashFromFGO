using MegaCrit.Sts2.Core.CardSelection;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Common;

public sealed class CelestialThrust() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class FieldReading() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("Discard", 1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        var selected = await CardSelectCmd.FromHandForDiscard(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this);
        await CardCmd.Discard(c, selected);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class PrayerToBishamonten() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 20)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class TurnTheReins() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 50), new DynamicVar("NpCharge", 50), new CardsVar(0)];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["Stars"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await CritStars.Spend(c, Owner.Creature, DynamicVars["Stars"].IntValue, this)) return;
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (DynamicVars.Cards.IntValue > 0) await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class BattleOrder() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var doctrine = Owner.Creature.GetPower<DoctrinePower>();
        if (doctrine == null) return;
        var candidate = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault(card =>
            card is IPreceptCard tagged && doctrine.WouldAdvanceAfter(Precept.Heaven, tagged.Precept));
        if (candidate != null) await CardPileCmd.Add(candidate, PileType.Hand);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class GeneralsCounsel() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class CommandersStaff() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move), new DynamicVar("KenshinBonus", 4)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var damage = DynamicVars.Damage.BaseValue + (Owner.Creature.HasPower<KenshinFormPower>() ? DynamicVars["KenshinBonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class ArmourIsInTheChest() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class DrinkAmongBullets() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<WeakPower>(c, p.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class KasugayamaGuard() : KagetoraCard(2, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5m);
}

public sealed class SixPlateCuirass() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class InterposeTheSpear() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move), new CardsVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var advances = Owner.Creature.GetPower<DoctrinePower>()?.WouldAdvance(Precept.Chest) == true;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (advances) await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class SaltForTheRival() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move), new DynamicVar("RemoveWeak", 1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        await CreatureCmd.GainBlock(target, DynamicVars.Block, p);
        if (target.GetPower<WeakPower>() is not { } weak) return;
        var remove = IsUpgraded ? (int)weak.Amount : DynamicVars["RemoveWeak"].IntValue;
        if (weak.Amount <= remove) await PowerCmd.Remove(weak);
        else await PowerCmd.ModifyAmount(c, weak, -remove, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class ClosedFormation() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move), new DynamicVar("EmptyBonus", 4)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var block = DynamicVars.Block.BaseValue + (Owner.Creature.Block <= 0 ? DynamicVars["EmptyBonus"].BaseValue : 0m);
        return CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, p);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(2m); DynamicVars["EmptyBonus"].UpgradeValueBy(1m); }
}

public sealed class EightPetalSpear() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class HoushoutsukigeCharge() : KagetoraCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move), new DynamicVar("Hits", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class StepOfVictory() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Feet)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 10)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class TurnTheFormation() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Feet)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 50), new DynamicVar("Stars", 50), new CardsVar(0)];
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, 50, this);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await NpCharge.Spend(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this)) return;
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        if (DynamicVars.Cards.IntValue > 0) await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class NaginataSweep() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) => await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).TargetingAllOpponents(Owner.Creature.CombatState!).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class AlternatingAttack() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", 2)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 2 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
