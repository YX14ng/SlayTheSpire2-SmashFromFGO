using AstolfoRider.AstolfoRiderCode.Caprice;
using AstolfoRider.AstolfoRiderCode.Cards.Special;
using AstolfoRider.AstolfoRiderCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Cards.Uncommon;

internal static class UncommonRules
{
    public static async Task<CommandType?> ChooseCaprice(
        PlayerChoiceContext context, CardModel source, bool remainingOnly = false, bool refill = false)
    {
        var owner = source.Owner;
        var state = owner?.Creature.CombatState;
        if (owner == null || state == null) return null;
        var types = remainingOnly
            ? Caprices.Remaining(owner.Creature)
            : new[] { CommandType.Quick, CommandType.Arts, CommandType.Buster };
        var options = types.Select(type => type switch
        {
            CommandType.Quick => state.CreateCard(ModelDb.Card<ChooseQuickCaprice>(), owner),
            CommandType.Arts => state.CreateCard(ModelDb.Card<ChooseArtsCaprice>(), owner),
            _ => state.CreateCard(ModelDb.Card<ChooseBusterCaprice>(), owner)
        }).Cast<CardModel>().Take(2 + (remainingOnly ? 0 : 1)).ToList();
        var selected = await CardSelectCmd.FromChooseACardScreen(context, options, owner, false);
        if (selected is not ICapriceChoice choice) return null;
        await Caprices.Choose(context, owner.Creature, choice.ChosenType, source, refill);
        return choice.ChosenType;
    }

    public static async Task ReplacePower<T>(
        PlayerChoiceContext context, Creature owner, decimal amount, CardModel source)
        where T : PowerModel
    {
        if (owner.GetPower<T>() is { } existing) await PowerCmd.Remove(existing);
        await PowerCmd.Apply<T>(context, owner, amount, owner, source);
    }
}

public sealed class EvaporatedReasonD() : AstolfoCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<EvaporatedReasonDPower>("Reason", 1m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<EvaporatedReasonDPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class ChooseWithoutThinking() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Exhaust, CardKeyword.Retain] : [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var chosen = await UncommonRules.ChooseCaprice(c, this);
        if (chosen.HasValue)
            await UncommonRules.ReplacePower<FreeCapriceCommandPower>(
                c, Owner.Creature, Caprices.Bit(chosen.Value), this);
    }
    protected override void OnUpgrade() { }
}

public sealed class ChangingCaprice() : AstolfoCard(
    0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if ((await UncommonRules.ChooseCaprice(c, this, remainingOnly: true)).HasValue)
            await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class FirstThingThatLooksFun() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var current = Caprices.Current(Owner.Creature);
        if (current.HasValue)
        {
            var chosen = (await CardSelectCmd.FromHand(c, Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 1),
                card => card != this && card is AstolfoCommandCard { IsNoblePhantasm: false }, this))
                .FirstOrDefault() as AstolfoCommandCard;
            chosen?.OverrideCommandType(current.Value);
        }
        await CardPileCmd.Draw(c, 1, Owner);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class PaladinsInstinct() : AstolfoCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PaladinsInstinctPower>("Block", 6m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<PaladinsInstinctPower>(c, Owner.Creature,
            DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(3m);
}

public sealed class BestRouteMaybe() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        var chosen = (await CardSelectCmd.FromHand(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), card => card != this, this)).FirstOrDefault();
        if (chosen == null) return;
        var matches = chosen is ICommandTyped typed && !typed.IsNoblePhantasm &&
                      Caprices.Matches(Owner.Creature, typed.CommandType);
        await CardPileCmd.Add([chosen], PileType.Draw, CardPilePosition.Top);
        if (matches) await NpCharge.Gain(c, Owner.Creature, 10, this);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class NoSecrets() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new DynamicVar("Discard", 2), new DynamicVar("StarsPer", 10), new DynamicVar("MaxStars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        var selected = await CardSelectCmd.FromHandForDiscard(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 2, 2), null, this);
        var gain = Math.Min(20, selected.Count(card => card is ICommandTyped typed &&
            !typed.IsNoblePhantasm && !Caprices.Matches(Owner.Creature, typed.CommandType)) * 10);
        await CardCmd.Discard(c, selected);
        if (gain > 0) await CritStars.Gain(c, Owner.Creature, gain, this);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class ContagiousGoodHumor() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, ValueProp.Move), new DynamicVar("LaterBlock", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        foreach (var player in Owner.Creature.CombatState?.PlayerCreatures.Where(x => !x.IsDead).ToList() ?? [])
            await CreatureCmd.GainBlock(player, DynamicVars.Block.BaseValue, ValueProp.Move, p);
        await PowerCmd.Apply<ContagiousGoodHumorPower>(
            c, Owner.Creature, DynamicVars["LaterBlock"].BaseValue, Owner.Creature, this, silent: true);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["LaterBlock"].UpgradeValueBy(1m);
    }
}

public sealed class RidingAPlus() : AstolfoCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<RidingAPlusPower>("Damage", 3m), new DynamicVar("Stars", 10)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<RidingAPlusPower>(c, Owner.Creature,
            DynamicVars["Damage"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Damage"].UpgradeValueBy(2m);
}

public sealed class IndependentActionB() : AstolfoCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<IndependentActionBPower>("Stars", 10m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<IndependentActionBPower>(c, Owner.Creature,
            DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class TripleQuick() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Quicks", 2), new DynamicVar("CritReady", 1), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (Caprices.QuicksPlayedThisTurn(Owner.Creature) >= 2)
            await Criticals.GrantReady(c, Owner.Creature, 1, this);
        else await CritStars.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class FeatherDance() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, CommandType.Quick)
{
    private const int Hits = 3;
    public override int DamagePortions => Hits;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue, Hits))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitCount(Hits)
            .OnlyPlayAnimOnce().WithHitFx("vfx/vfx_starry_impact").Execute(c);
        await CritStars.Gain(c, Owner.Creature, 10, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class ZigzagCharge() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, CommandType.Quick)
{
    private const int Hits = 2;
    public override int DamagePortions => Hits;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var fulfills = MatchesCaprice;
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue, Hits))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitCount(Hits)
            .OnlyPlayAnimOnce().WithHitFx("vfx/vfx_starry_impact").Execute(c);
        if (fulfills) await CritStars.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class LuckAPlus() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 30)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        var chosen = (await CardSelectCmd.FromHand(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), card => card != this, this)).FirstOrDefault();
        chosen?.GiveSingleTurnRetain();
    }
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class IrresistibleImpulse() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 30), new EnergyVar(1), new CardsVar(1)];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await CritStars.Spend(c, Owner.Creature, DynamicVars["StarCost"].IntValue, this)) return;
        await PlayerCmd.GainEnergy(1, Owner);
        await CardPileCmd.Draw(c, 1, Owner);
    }
    protected override void OnUpgrade() => DynamicVars["StarCost"].UpgradeValueBy(-10m);
}

public sealed class UnstableTakeoff() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 50), new DynamicVar("Evasion", 1), new BlockVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (await CritStars.Spend(c, Owner.Creature, DynamicVars["StarCost"].IntValue, this))
            await Evasion.Grant(c, Owner.Creature, 1, this);
        else await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["StarCost"].UpgradeValueBy(-10m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}

public sealed class DimensionalLeap() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(9m, ValueProp.Move), new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (Evasion.Of(Owner.Creature) > 0) await NpCharge.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class ImpossibleExistence() : AstolfoCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ImpossibleExistencePower>("Stars", 20m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<ImpossibleExistencePower>(c, Owner.Creature,
            DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class WingsOverTheGroup() : AstolfoCard(
    2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(9m, ValueProp.Move), new DynamicVar("EvasionBonus", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var amount = DynamicVars.Block.BaseValue +
                     (Evasion.Of(Owner.Creature) > 0 ? DynamicVars["EvasionBonus"].BaseValue : 0m);
        foreach (var player in Owner.Creature.CombatState?.PlayerCreatures.Where(x => !x.IsDead).ToList() ?? [])
            await CreatureCmd.GainBlock(player, amount, ValueProp.Move, p);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class DiveFromAnotherWorld() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, CommandType.Quick)
{
    public override int DamagePortions => Math.Max(1, Owner?.Creature.CombatState?.HittableEnemies.Count ?? 1);
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("EvasionCost", 1), new DynamicVar("Bonus", 8)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var spent = await Evasion.Spend(c, Owner.Creature, 1, this);
        var targets = Math.Max(1, Owner.Creature.CombatState!.HittableEnemies.Count);
        var damage = DynamicVars.Damage.BaseValue + (spent ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(WithCapriceDamage(damage, targets)).FromCardFgoCompatibility(this, p)
            .TargetingAllOpponents(Owner.Creature.CombatState!).WithHitFx("vfx/vfx_starry_impact")
            .SpawningHitVfxOnEachCreature().Execute(c);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}

public sealed class Reappearance() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, CommandType.Buster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("EvasionBonus", 6)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var damage = DynamicVars.Damage.BaseValue +
                     (Evasion.Of(Owner.Creature) > 0 ? DynamicVars["EvasionBonus"].BaseValue : 0m);
        await DamageCmd.Attack(WithCapriceDamage(damage)).FromCardFgoCompatibility(this, p)
            .Targeting(p.Target).WithHitFx("vfx/vfx_heavy_blunt").Execute(c);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["EvasionBonus"].UpgradeValueBy(2m);
    }
}

public sealed class TouchAndKnockDownD() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, CommandType.Buster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(11m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 2m), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var alreadyWeak = p.Target.GetPowerAmount<WeakPower>() > 0;
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, p).Targeting(p.Target)
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(c);
        if (!p.Target.IsDead)
            await PowerCmd.Apply<WeakPower>(c, p.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        if (alreadyWeak) await CritStars.Gain(c, Owner.Creature, 10, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Weak"].UpgradeValueBy(1m);
    }
}

public sealed class LunaBreakManual() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Debuffs", 1), new PowerVar<ArtifactPower>("Artifact", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await Cleanse.RemoveDebuffs(Owner.Creature, 1);
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature,
            DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["Artifact"].UpgradeValueBy(1m);
}

public sealed class PagesToTheWind() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Buffs", 1), new BlockVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var removed = await Cleanse.RemoveOffensiveBuffs(p.Target, 1);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (removed > 0) await NpCharge.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class BlackLuna() : AstolfoCommandCard(
    2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, CommandType.Buster)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var targets = Owner.Creature.CombatState?.HittableEnemies.ToList() ?? [];
        var portions = Math.Max(1, targets.Count);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue, portions))
            .FromCardFgoCompatibility(this, p).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_heavy_blunt").SpawningHitVfxOnEachCreature().Execute(c);
        foreach (var enemy in targets.Where(x => !x.IsDead))
            await PowerCmd.Apply<WeakPower>(c, enemy, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class HomunculusRescue() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Move), new DynamicVar("Debuffs", 1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        await CreatureCmd.GainBlock(target, DynamicVars.Block, p);
        if (target.CurrentHp * 2 <= target.MaxHp) await Cleanse.RemoveDebuffs(target, 1);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class SharedAdventure() : AstolfoCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new BlockVar(5m, ValueProp.Move), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        if (target.Player != null) await CardPileCmd.Draw(c, 1, target.Player);
        await CreatureCmd.GainBlock(target, DynamicVars.Block, p);
        await CritStars.Gain(c, Owner.Creature, 10, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class DifferentAdventureEveryTime() : AstolfoCard(
    2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<DifferentAdventureEveryTimePower>("Adventure", 1m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<DifferentAdventureEveryTimePower>(c, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
