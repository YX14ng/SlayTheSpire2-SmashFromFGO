using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Cards.Basic;

public sealed class Buster() : ShutenCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, ShutenStyle.Caster), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt").Execute(context);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Arts() : ShutenCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("NpCharge", 30)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(context);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Quick() : ShutenCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, ShutenStyle.Assassin), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact").Execute(context);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Defender() : ShutenCard(
    1, CardType.Skill, CardRarity.Basic, TargetType.Self, ShutenStyle.Caster)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class FruityWineAroma() : ShutenCard(
    1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy, ShutenStyle.Assassin), BaseLib.Abstracts.ITranscendenceCard
{
    public MegaCrit.Sts2.Core.Models.CardModel GetTranscendenceTransformedCard() =>
        MegaCrit.Sts2.Core.Models.ModelDb.Card<Rare.FruityAromaEx>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PoisonPower>("Poison", 4m), new DynamicVar("Sake", 20)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<SakePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<PoisonPower>(context, cardPlay.Target,
            DynamicVars["Poison"].BaseValue, Owner.Creature, this);
        await SakeBank.Gain(context, Owner.Creature, DynamicVars["Sake"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Poison"].UpgradeValueBy(2m);
}
