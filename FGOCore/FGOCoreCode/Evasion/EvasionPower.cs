using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Evasion;

/// <summary>Notification emitted once a real enemy hit consumes Evasion.</summary>
public readonly record struct EvasionConsumed(Creature Owner);

/// <summary>Powers and relics can react after an Evasion charge is consumed.</summary>
public interface IEvasionConsumedListener
{
    Task AfterEvasionConsumed(PlayerChoiceContext context, EvasionConsumed evasion);
}

/// <summary>
/// Shared FGO Evasion: prevent one enemy Attack hit that would otherwise reach HP.
/// Block resolves before HP-loss hooks and Buffer is deliberately allowed to resolve first.
/// Each damaging hit consumes one charge; self-loss and environmental damage pass through.
/// </summary>
public sealed class EvasionPower : FGOCorePower
{
    public const int MaxStacks = 3;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;

    private bool _isClamping;

    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || amount <= 0m) return amount;
        if (!props.IsPoweredAttack() || dealer?.Monster == null) return amount;

        // Buffer also uses the late phase. Explicitly yielding while it exists makes the order
        // independent from hook-listener registration: Evasion never disappears behind Buffer.
        if (Owner.GetPower<BufferPower>() is { Amount: > 0 }) return amount;

        return 0m;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        var context = new BlockingPlayerChoiceContext();
        await PowerCmd.Decrement(this);
        await Listeners.ForEachListener<IEvasionConsumedListener>(
            Owner, listener => listener.AfterEvasionConsumed(context, new EvasionConsumed(Owner)));
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        if (power != this || _isClamping || Amount <= MaxStacks) return;

        _isClamping = true;
        await PowerCmd.ModifyAmount(
            choiceContext, this, MaxStacks - Amount, applier ?? Owner, cardSource);
        _isClamping = false;
    }
}

/// <summary>Single entry point that grants Evasion while respecting its global cap.</summary>
public static class Evasion
{
    public static int Of(Creature creature) => (int)creature.GetPowerAmount<EvasionPower>();

    public static Task Grant(Creature creature, int amount, CardModel? source = null) =>
        Grant(new BlockingPlayerChoiceContext(), creature, amount, source);

    public static async Task Grant(
        PlayerChoiceContext context,
        Creature creature,
        int amount,
        CardModel? source = null)
    {
        if (amount <= 0) return;
        var room = EvasionPower.MaxStacks - Of(creature);
        if (room <= 0) return;
        await PowerCmd.Apply<EvasionPower>(
            context, creature, Math.Min(amount, room), creature, source);
    }

    public static async Task<bool> Spend(
        PlayerChoiceContext context,
        Creature creature,
        int amount,
        CardModel? source = null)
    {
        if (amount <= 0 || Of(creature) < amount) return false;
        var power = creature.GetPower<EvasionPower>();
        if (power == null) return false;
        await PowerCmd.ModifyAmount(context, power, -amount, creature, source);
        return true;
    }
}
