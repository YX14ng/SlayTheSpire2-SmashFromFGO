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
using ShutenDouji.ShutenDoujiCode.Cards.Special;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Cards.Common;

internal static class CommonCardRules
{
    public static bool HasDebuff(MegaCrit.Sts2.Core.Entities.Creatures.Creature creature) =>
        creature.GetPowerInstances<PowerModel>().Any(power => power.Type == PowerType.Debuff);
}

public sealed class PoisonedNeedle() : ShutenCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new PowerVar<PoisonPower>("Poison", 3m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(context);
        await PowerCmd.Apply<PoisonPower>(context, cardPlay.Target, DynamicVars["Poison"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Poison"].UpgradeValueBy(1m);
    }
}

public sealed class OniClaw() : ShutenCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("PoisonBonus", 3), new DynamicVar("NpCharge", 10)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Any(enemy => enemy.HasPower<PoisonPower>()) == true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var poisoned = cardPlay.Target.HasPower<PoisonPower>();
        var damage = DynamicVars.Damage.BaseValue + (poisoned ? DynamicVars["PoisonBonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(context);
        if (poisoned)
            await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["PoisonBonus"].UpgradeValueBy(1m);
    }
}

public sealed class IntoxicatingBreath() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 4m), new DynamicVar("Sake", 10)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<PoisonPower>(context, cardPlay.Target, DynamicVars["Poison"].BaseValue,
            Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(2m);
}

public sealed class BanquetGaze() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(context, cardPlay.Target, DynamicVars["Weak"].BaseValue,
            Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

public sealed class PavilionMist() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<WeakPower>(context, cardPlay.Target, DynamicVars["Weak"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class SpilledWine() : ShutenCard(
    0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies, ShutenStyle.Assassin)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 3m), new DynamicVar("Sake", 10)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState!.HittableEnemies)
            await PowerCmd.Apply<PoisonPower>(context, enemy, DynamicVars["Poison"].BaseValue,
                Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Poison"].UpgradeValueBy(1m);
        DynamicVars["Sake"].UpgradeValueBy(10m);
    }
}

public sealed class PresenceLessStep() : ShutenCard(
    0, CardType.Skill, CardRarity.Common, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new DynamicVar("Discard", 1), new DynamicVar("Sake", 10)];
    protected override bool ShouldGlowGoldInternal => HasCross;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var crossed = HasCross;
        await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
        var selected = await CardSelectCmd.FromHandForDiscard(context, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this);
        await CardCmd.Discard(context, selected);
        if (crossed)
            await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Sake"].UpgradeValueBy(10m);
}

public sealed class MidnightCup() : ShutenCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 20), new DynamicVar("Sake", 10)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Any(CommonCardRules.HasDebuff) == true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var debuffed = CommonCardRules.HasDebuff(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(context);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (debuffed)
            await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class DangerousGift() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 3m), new BlockVar(5m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];
    protected override bool ShouldGlowGoldInternal => HasCross;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var crossed = HasCross;
        await PowerCmd.Apply<PoisonPower>(context, cardPlay.Target, DynamicVars["Poison"].BaseValue,
            Owner.Creature, this);
        if (crossed) await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Poison"].UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}

public sealed class RedInvitation() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<VulnerablePower>("Vulnerable", 2m), new DynamicVar("Sake", 10)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<VulnerablePower>(context, cardPlay.Target,
            DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Vulnerable"].UpgradeValueBy(1m);
}

public sealed class GohoClub() : ShutenCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move), new DynamicVar("SakeCost", 20), new DynamicVar("Bonus", 5)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var damage = DynamicVars.Damage.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_heavy_blunt").Execute(context);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}

public sealed class CorrectiveKick() : ShutenCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    private const int Hits = 2;
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("SakeCost", 20), new DynamicVar("Bonus", 1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var damage = DynamicVars.Damage.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_starry_impact").Execute(context);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class MagicLesson() : ShutenCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 20), new DynamicVar("SakeCost", 10), new CardsVar(1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_lightning").Execute(context);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this))
            await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class HakuGuard() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, ValueProp.Move), new DynamicVar("SakeCost", 10), new DynamicVar("Bonus", 4)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var block = DynamicVars.Block.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}

public sealed class WhiteAmulet() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, ValueProp.Move), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class GohoSweep() : ShutenCard(
    1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, ShutenStyle.Caster), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("SakeCost", 20), new DynamicVar("Bonus", 3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var damage = DynamicVars.Damage.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState!).WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(context);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}

public sealed class VisceraGrip() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Any(CommonCardRules.HasDebuff) == true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var debuffed = CommonCardRules.HasDebuff(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(context, cardPlay.Target, DynamicVars["Weak"].BaseValue,
            Owner.Creature, this);
        if (debuffed)
            await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

public sealed class GuardDrink() : ShutenCard(
    2, CardType.Skill, CardRarity.Common, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(14m, ValueProp.Move), new DynamicVar("SakeCost", 20), new DynamicVar("Bonus", 6)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var block = DynamicVars.Block.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}

public sealed class MountOoeRecipe() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Sake", 30), new DynamicVar("SakeCost", 20), new PowerVar<PoisonPower>("Poison", 8m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<SakePower>(), HoverTipFactory.FromPower<PoisonPower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var state = Owner.Creature.CombatState;
        if (state == null) return;

        var options = new List<CardModel>
        {
            state.CreateCard(ModelDb.Card<BrewMountOoeSake>(), Owner)
        };
        if (SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue))
            options.Add(state.CreateCard(ModelDb.Card<DistillMountOoePoison>(), Owner));

        var selected = await CardSelectCmd.FromChooseACardScreen(context, options, Owner, false);
        if (selected is not IMountOoeRecipeChoice choice) return;
        if (choice.BrewSake)
        {
            await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
            return;
        }

        if (await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this))
            await PowerCmd.Apply<PoisonPower>(context, cardPlay.Target,
                DynamicVars["Poison"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Sake"].UpgradeValueBy(10m);
        DynamicVars["Poison"].UpgradeValueBy(3m);
    }
}

public sealed class SharedToast() : ShutenCard(
    1, CardType.Skill, CardRarity.Common, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(4m, ValueProp.Move), new DynamicVar("CrossBonus", 4)];
    protected override bool ShouldGlowGoldInternal => HasCross;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var block = DynamicVars.Block.BaseValue + (HasCross ? DynamicVars["CrossBonus"].BaseValue : 0m);
        foreach (var creature in Owner.Creature.CombatState!.PlayerCreatures)
            await CreatureCmd.GainBlock(creature, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["CrossBonus"].UpgradeValueBy(1m);
    }
}
