using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using ShutenDouji.ShutenDoujiCode.Powers;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Cards.Uncommon;

internal static class UncommonRules
{
    public static bool HasDebuff(MegaCrit.Sts2.Core.Entities.Creatures.Creature creature) =>
        creature.Powers.Any(power => power.TypeForCurrentAmount == PowerType.Debuff && power is not IResourcePower);
}

public sealed class FruityWineAromaAPlus() : ShutenCard(
    2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 4m), new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<PoisonPower>(context, enemy, DynamicVars["Poison"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<WeakPower>(context, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        }
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(2m);
}

public sealed class SevenColorPoison() : ShutenCard(
    2, CardType.Skill, CardRarity.Uncommon, TargetType.RandomEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 4m), new RepeatVar(3), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            var enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            if (enemy != null)
                await PowerCmd.Apply<PoisonPower>(context, enemy, DynamicVars["Poison"].BaseValue,
                    Owner.Creature, this);
        }
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Repeat.UpgradeValueBy(1m);
}

public sealed class PresenceConcealmentC() : ShutenCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PresenceConcealmentPower>("Stars", 10m), new DynamicVar("Sake", 10)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<PresenceConcealmentPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<PresenceConcealmentPower>(context, Owner.Creature,
                DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class OniSpeciesMagicA() : ShutenCard(
    2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Assassin)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<StrengthPower>("Strength", 1m), new DynamicVar("Overcharge", 1), new DynamicVar("Sake", 30)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<OverchargeBlessingPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        foreach (var creature in Owner.Creature.CombatState!.PlayerCreatures)
        {
            var strength = creature == Owner.Creature ? DynamicVars["Strength"].BaseValue : 1m;
            await PowerCmd.Apply<StrengthPower>(context, creature, strength, Owner.Creature, this);
        }
        await PowerCmd.Apply<OverchargeBlessingPower>(context, Owner.Creature, 1m, Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Strength"].UpgradeValueBy(1m);
}

public sealed class HeadlessOni() : ShutenCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GutsPower>()];
    protected override bool ShouldGlowGoldInternal => Owner.Creature.CurrentHp * 2 <= Owner.Creature.MaxHp;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_bloody_impact").Execute(context);
        if (Owner.Creature.CurrentHp * 2 <= Owner.Creature.MaxHp)
            await PowerCmd.Apply<GutsPower>(context, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}

public sealed class InvitationToPerdition() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<InvitationToPerditionPower>("StrengthLoss", 2m), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<InvitationToPerditionPower>(context, cardPlay.Target,
            DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["StrengthLoss"].UpgradeValueBy(1m);
}

public sealed class PoisonedTable() : ShutenCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonedTablePower>("Poison", 2m), new DynamicVar("Threshold", 30)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<PoisonedTablePower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<PoisonedTablePower>(context, Owner.Creature,
                DynamicVars["Poison"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(1m);
}

public sealed class AntiquitiesCollection() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new DynamicVar("PutBack", 1), new DynamicVar("Sake", 10)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
        var selected = await CardSelectCmd.FromHand(context, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars["PutBack"].IntValue), null, this);
        if (selected.Any()) await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class BoneCollector() : ShutenCard(
    1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    private const int Hits = 3;
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("DebuffBonus", 1)];
    protected override bool ShouldGlowGoldInternal => CombatState?.HittableEnemies.Any(UncommonRules.HasDebuff) == true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = DynamicVars.Damage.BaseValue +
                     (UncommonRules.HasDebuff(cardPlay.Target) ? DynamicVars["DebuffBonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_bloody_impact").Execute(context);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["DebuffBonus"].UpgradeValueBy(1m);
    }
}

public sealed class FalseSweetness() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 6m), new DynamicVar("Artifact", 1), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<ArtifactPower>(), HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => CombatState?.HittableEnemies.Any(e => e.HasPower<ArtifactPower>()) == true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (cardPlay.Target.GetPower<ArtifactPower>() is { } artifact)
        {
            if (artifact.Amount <= 1) await PowerCmd.Remove(artifact);
            else await PowerCmd.ModifyAmount(context, artifact, -1m, Owner.Creature, this);
            await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
        }
        else
        {
            await PowerCmd.Apply<PoisonPower>(context, cardPlay.Target, DynamicVars["Poison"].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(2m);
}

public sealed class MountOoeHaze() : ShutenCard(
    2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(12m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        foreach (var enemy in Owner.Creature.CombatState!.HittableEnemies)
            await PowerCmd.Apply<WeakPower>(context, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class DemonicWhisper() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>("Weak", 2m), new CardsVar(1)];
    protected override bool ShouldGlowGoldInternal => HasCross;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var crossed = HasCross;
        await PowerCmd.Apply<WeakPower>(context, cardPlay.Target, DynamicVars["Weak"].BaseValue,
            Owner.Creature, this);
        if (crossed) await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

public sealed class LastDrop() : ShutenCard(
    0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Assassin)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<NextPoisonAmplifierPower>("PoisonBonus", 5m), new DynamicVar("Sake", 10)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!Owner.Creature.HasPower<NextPoisonAmplifierPower>())
            await PowerCmd.Apply<NextPoisonAmplifierPower>(context, Owner.Creature,
                DynamicVars["PoisonBonus"].BaseValue, Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PoisonBonus"].UpgradeValueBy(3m);
        DynamicVars["Sake"].UpgradeValueBy(10m);
    }
}

public sealed class DaughterOfTheDragonGod() : ShutenCard(
    2, CardType.Power, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Assassin)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<DragonGodDaughterPower>("Poison", 1m), new DynamicVar("Cap", 3)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<DragonGodDaughterPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<DragonGodDaughterPower>(context, Owner.Creature,
                DynamicVars["Poison"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(1m);
}

public sealed class GohoOniMortalGripAPlus() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>("Weak", 2m), new DynamicVar("SakeCost", 20), new PowerVar<VulnerablePower>("Vulnerable", 2m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<SakePower>()];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(context, cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        if (await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this))
            await PowerCmd.Apply<VulnerablePower>(context, cardPlay.Target,
                DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

public sealed class SlaughterClubB() : ShutenCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    private const int Hits = 3;
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("SakeCost", 30), new DynamicVar("Bonus", 2)];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var damage = DynamicVars.Damage.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitCount(Hits).OnlyPlayAnimOnce()
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(context);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class ArtsBusterReinforcement() : ShutenCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("ArtsNp", 10), new DynamicVar("BusterDamage", 3)];

    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<ArtsBusterReinforcementPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<ArtsBusterReinforcementPower>(context, Owner.Creature,
                IsUpgraded ? 2m : 1m, Owner.Creature, this);

    protected override void OnUpgrade()
    {
        DynamicVars["ArtsNp"].UpgradeValueBy(10m);
        DynamicVars["BusterDamage"].UpgradeValueBy(1m);
    }
}

public sealed class OniMagicProtectionA() : ShutenCard(
    2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Caster)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<StrengthPower>("Strength", 2m), new PowerVar<ArtifactPower>("Artifact", 1m), new DynamicVar("SakeCost", 20), new DynamicVar("AllyStrength", 1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<ArtifactPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(context, Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<ArtifactPower>(context, Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
        if (!await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this)) return;
        foreach (var ally in Owner.Creature.CombatState!.PlayerCreatures.Where(c => c != Owner.Creature))
            await PowerCmd.Apply<StrengthPower>(context, ally, DynamicVars["AllyStrength"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Strength"].UpgradeValueBy(1m);
}

public sealed class AccurateStrike() : ShutenCard(
    0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Caster)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SureHitPower>("SureHit", 1m), new DynamicVar("Sake", 10), new CardsVar(0)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<SureHitPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await SureHit.Grant(context, Owner.Creature, 1, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
        if (DynamicVars.Cards.IntValue > 0) await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class WhiteFamiliarGuard() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Caster)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(10m, ValueProp.Move), new DynamicVar("SakeCost", 20), new DynamicVar("Bonus", 10)];
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
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}

public sealed class DemonSlayingBlow() : ShutenCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(16m, ValueProp.Move), new DynamicVar("NpCharge", 20), new DynamicVar("Bonus", 7)];
    protected override bool ShouldGlowGoldInternal =>
        Owner.RunState.CurrentRoom?.RoomType is RoomType.Elite or RoomType.Boss ||
        CombatState?.HittableEnemies.Any(UncommonRules.HasDebuff) == true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var qualifies = UncommonRules.HasDebuff(cardPlay.Target) ||
                        Owner.RunState.CurrentRoom?.RoomType is RoomType.Elite or RoomType.Boss;
        var damage = DynamicVars.Damage.BaseValue + (qualifies ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_heavy_blunt").Execute(context);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}

public sealed class MagicalGirlSteps() : ShutenCard(
    1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    private const int Hits = 3;
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("SakeCost", 20), new DynamicVar("Bonus", 1)];
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

public sealed class OniReprimand() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Buffs", 1), new BlockVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Cleanse.RemoveOffensiveBuffs(cardPlay.Target, DynamicVars["Buffs"].IntValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Buffs"].UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}

public sealed class HakuWarning() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(9m, ValueProp.Move), new DynamicVar("Debuffs", 1), new DynamicVar("SakeCost", 20), new DynamicVar("Bonus", 5)];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var target = cardPlay.Target ?? Owner.Creature;
        var empowered = SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);
        var block = DynamicVars.Block.BaseValue + (empowered ? DynamicVars["Bonus"].BaseValue : 0m);
        await CreatureCmd.GainBlock(target, block, ValueProp.Move, cardPlay);
        await Cleanse.RemoveDebuffs(target, DynamicVars["Debuffs"].IntValue);
        if (empowered) await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}

public sealed class CauldronSweep() : ShutenCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, ShutenStyle.Caster), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new PowerVar<PoisonPower>("Poison", 3m), new DynamicVar("NpCharge", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var enemies = Owner.Creature.CombatState!.HittableEnemies.ToList();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay)
            .TargetingAllOpponents(Owner.Creature.CombatState).WithHitFx("vfx/vfx_attack_lightning").Execute(context);
        foreach (var enemy in enemies.Where(e => !e.IsDead))
            await PowerCmd.Apply<PoisonPower>(context, enemy, DynamicVars["Poison"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Poison"].UpgradeValueBy(1m);
    }
}

public sealed class GohoLesson() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new DynamicVar("Discard", 1), new DynamicVar("Sake", 20)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
        var selected = (await CardSelectCmd.FromHandForDiscard(context, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this)).FirstOrDefault();
        if (selected == null) return;
        var assassin = selected is IShutenStyleCard { Style: ShutenStyle.Assassin };
        await CardCmd.Discard(context, [selected]);
        if (assassin) await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class BreakDivineProtection() : ShutenCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Caster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("DamageBonus", 5), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        Owner.Creature.HasPower<DivineProtectionBreakerPower>()
            ? Task.CompletedTask
            : PowerCmd.Apply<DivineProtectionBreakerPower>(context, Owner.Creature,
                IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade()
    {
        DynamicVars["DamageBonus"].UpgradeValueBy(2m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}

public sealed class BattleContinuationAPlus() : ShutenCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, ShutenStyle.Caster)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<GutsPower>("Guts", 1m), new DynamicVar("SakeCost", 30), new HealVar(6m)];
    protected override bool ShouldGlowGoldInternal => SakeBank.CanSpend(Owner.Creature, DynamicVars["SakeCost"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GutsPower>(context, Owner.Creature, 1m, Owner.Creature, this);
        if (await SakeBank.Spend(context, Owner.Creature, DynamicVars["SakeCost"].IntValue, this))
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SakeCost"].UpgradeValueBy(-10m);
        DynamicVars.Heal.UpgradeValueBy(3m);
    }
}
