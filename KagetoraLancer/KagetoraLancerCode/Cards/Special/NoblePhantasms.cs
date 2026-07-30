using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Special;

public interface IKagetoraNpCard : ICommandTyped { }

public abstract class KagetoraNpCard() : KagetoraCard(
    0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy), IKagetoraNpCard
{
    public const int ChargeCost = 100;
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    protected override bool ShouldGlowGoldInternal => IsPlayable;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BishamontenBlessingPower>()];

    protected async Task<int> ConsumeEffectiveTier(PlayerChoiceContext context)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(context, Owner.Creature, ChargeCost, this);
        return Math.Clamp(tier, ChargeCost, 500);
    }

    protected static Task RemoveOffensiveBuffs(
        MegaCrit.Sts2.Core.Entities.Creatures.Creature target) =>
        Cleanse.RemoveOffensiveBuffs(target);
}

public sealed class BitenHassouKurumaGakariNoJin() : KagetoraNpCard
{
    private const int Hits = 8;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Hits", Hits)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;
        var tier = await ConsumeEffectiveTier(context);
        var level = Math.Clamp(NpLevels.Get(Owner), 1, NpLevels.MaxLevel(Owner));
        var perHit = 3m + level;

        for (var hit = 0; hit < Hits && !target.IsDead; hit++)
        {
            VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_dramatic_stab");
            await CreatureCmdCompatibility.Damage(
                context, target, perHit, ValueProp.Move, Owner.Creature, this, cardPlay);
        }

        // El primer NP limpia después de infligir daño y transforma aunque el objetivo haya muerto.
        await RemoveOffensiveBuffs(target);
        if (!target.IsDead)
        {
            var oc = Math.Clamp(tier / 100, 1, 5);
            var weak = oc >= 5 ? 3m : oc >= 3 ? 2m : 1m;
            await PowerCmd.Apply<WeakPower>(context, target, weak, Owner.Creature, this);
        }

        if (!Owner.Creature.HasPower<KenshinFormPower>())
        {
            await FormSwitch.Enter<KenshinFormPower>(context, Owner.Creature, this);
            await Listeners.ForEachListener<IAscensionListener>(Owner.Creature,
                listener => listener.AfterAscendingToKenshin(context, this));
        }
    }
}

public sealed class BitenHassouShiranui() : KagetoraNpCard
{
    private const int Hits = 4;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("AntiMan", 3)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;
        var tier = await ConsumeEffectiveTier(context);
        var level = Math.Clamp(NpLevels.Get(Owner), 1, NpLevels.MaxLevel(Owner));
        var perHit = 5m + 2m * level;
        perHit += Math.Clamp(tier / 100 - 1, 0, 4);
        if (FgoAttributes.Is(target, FgoAttribute.Man)) perHit += 3m;

        await RemoveOffensiveBuffs(target);
        for (var hit = 0; hit < Hits && !target.IsDead; hit++)
        {
            VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_dramatic_stab");
            await CreatureCmdCompatibility.Damage(
                context, target, perHit, ValueProp.Move, Owner.Creature, this, cardPlay);
        }
    }
}
