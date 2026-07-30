using AstolfoRider.AstolfoRiderCode.Caprice;
using AstolfoRider.AstolfoRiderCode.Cards.Special;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Cards.Basic;

public sealed class Buster() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, CommandType.Buster)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab").Execute(context);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Arts() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, CommandType.Arts)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("NpCharge", 30)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(context);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Quick() : AstolfoCommandCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, CommandType.Quick)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(WithCapriceDamage(DynamicVars.Damage.BaseValue))
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact").Execute(context);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Defender() : AstolfoCard(
    1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class PaladinsHunch() : AstolfoCard(
    1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var state = Owner.Creature.CombatState;
        if (state != null)
        {
            var options = new List<CardModel>
            {
                state.CreateCard(ModelDb.Card<ChooseQuickCaprice>(), Owner),
                state.CreateCard(ModelDb.Card<ChooseArtsCaprice>(), Owner),
                state.CreateCard(ModelDb.Card<ChooseBusterCaprice>(), Owner)
            };
            var selected = await CardSelectCmd.FromChooseACardScreen(context, options, Owner, false);
            if (selected is ICapriceChoice choice)
                await Caprices.Choose(context, Owner.Creature, choice.ChosenType, this);
        }
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
