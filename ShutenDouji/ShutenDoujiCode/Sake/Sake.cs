using FGOCore.FGOCoreCode.Cleanse;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ShutenDouji.ShutenDoujiCode.Powers;

namespace ShutenDouji.ShutenDoujiCode.Sake;

public readonly record struct SakeChange(Creature Owner, int Amount, CardModel? Source);

public interface ISakeGainedListener
{
    Task AfterSakeGained(PlayerChoiceContext context, SakeChange change);
}

public interface ISakeSpentListener
{
    Task AfterSakeSpent(PlayerChoiceContext context, SakeChange change);
}

public sealed class SakePower : ShutenPower, IResourcePower
{
    public const int Max = 100;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
}

public static class Sake
{
    public static int Current(Creature owner) => (int)owner.GetPowerAmount<SakePower>();
    public static bool CanSpend(Creature owner, int amount) => amount >= 0 && Current(owner) >= amount;

    public static async Task<int> Gain(
        PlayerChoiceContext context, Creature owner, int amount, CardModel? source = null)
    {
        var actual = Math.Clamp(amount, 0, SakePower.Max - Current(owner));
        if (actual <= 0) return 0;

        await PowerCmd.Apply<SakePower>(context, owner, actual, owner, source);
        var change = new SakeChange(owner, actual, source);
        await Listeners.ForEachListener<ISakeGainedListener>(owner,
            listener => listener.AfterSakeGained(context, change));
        return actual;
    }

    public static async Task<bool> Spend(
        PlayerChoiceContext context, Creature owner, int amount, CardModel? source = null)
    {
        if (amount <= 0) return true;
        var power = owner.GetPower<SakePower>();
        if (power == null || power.Amount < amount) return false;

        await PowerCmd.ModifyAmount(context, power, -amount, owner, source);
        var change = new SakeChange(owner, amount, source);
        await Listeners.ForEachListener<ISakeSpentListener>(owner,
            listener => listener.AfterSakeSpent(context, change));
        return true;
    }

    public static async Task<int> SpendUpTo(
        PlayerChoiceContext context, Creature owner, int maximum, CardModel? source = null)
    {
        var actual = Math.Min(Math.Max(0, maximum), Current(owner));
        actual -= actual % 10;
        if (actual <= 0) return 0;
        return await Spend(context, owner, actual, source) ? actual : 0;
    }

    public static Task<int> Gain(Creature owner, int amount, CardModel? source = null) =>
        Gain(new BlockingPlayerChoiceContext(), owner, amount, source);
}
