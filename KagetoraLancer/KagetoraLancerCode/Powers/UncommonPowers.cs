using KagetoraLancer.KagetoraLancerCode.Doctrine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

public sealed class BulletCurtainPower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterDamageReceived(PlayerChoiceContext context, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || dealer.Side == Owner.Side ||
            !props.IsPoweredAttack() || result.UnblockedDamage > 0) return;
        await CritStars.Gain(context, Owner, (int)Amount, null);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class SereneCounterPower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterDamageReceived(PlayerChoiceContext context, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || dealer.Side == Owner.Side || !props.IsPoweredAttack()) return;
        await CreatureCmdCompatibility.Damage(context, dealer, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, Owner, null, null);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class FearlessChestPower : KagetoraPower, IDoctrineAdvanceListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (!result.Advanced || result.Attempted != Precept.Chest) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

public sealed class JustPathPower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        // `?? 0` obligatorio: `null < 2` es FALSE en C# (toda comparación relacional levantada con
        // null lo es), así que sin DoctrinePower este power NO retornaba y regalaba el Bloqueo sin
        // condición — el mismo error de razonamiento que KagetoraUsages.WasUsed, con signo opuesto.
        if (!participants.Contains(Owner) || (Owner.GetPower<DoctrinePower>()?.AdvancesThisTurn ?? 0) < 2) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

public sealed class RidingPower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.Riding) ||
            cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card is not ICommandTyped { CommandType: CommandType.Quick, IsNoblePhantasm: false }) return;
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.Riding, cardPlay.Card);
        Flash();
        await CritStars.Gain(context, Owner, (int)Amount, cardPlay.Card);
    }
}

public sealed class GeneralsDoctrinePower : KagetoraPower, IDoctrineAdvanceListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.GeneralsDoctrine) ||
            result.Advanced || result.Attempted == Precept.None) return;
        await KagetoraUsages.Mark(
            context, Owner, KagetoraUsage.GeneralsDoctrine, result.CardPlay.Card);
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

public sealed class DivinityPower : KagetoraPower
{
    private CardPlay? _active;
    private bool _usedHit;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            _active = cardPlay;
            _usedHit = false;
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || cardSource?.Type != CardType.Attack || !props.IsPoweredAttack()) return 0m;
        if (_active == null) return Amount + (Owner.HasPower<KenshinFormPower>() ? 2m : 0m);
        if (_usedHit || _active.Card != cardSource || (cardPlay != null && cardPlay != _active)) return 0m;
        return Amount + (Owner.HasPower<KenshinFormPower>() ? 2m : 0m);
    }

    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (!_usedHit && _active != null && dealer == Owner && _active.Card == cardSource &&
            props.IsPoweredAttack())
            _usedHit = true;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_active == cardPlay) { _active = null; _usedHit = false; }
        return Task.CompletedTask;
    }
}
