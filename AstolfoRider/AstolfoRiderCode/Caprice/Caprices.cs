using AstolfoRider.AstolfoRiderCode.Cards;
using AstolfoRider.AstolfoRiderCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace AstolfoRider.AstolfoRiderCode.Caprice;

public readonly record struct CapriceDraw(Creature Owner, CommandType Type);
public readonly record struct CapriceFulfillment(
    Creature Owner, CardModel Card, CommandType Type, int NumberThisTurn);

public interface ICapriceDrawListener
{
    Task AfterCapriceDrawn(PlayerChoiceContext context, CapriceDraw draw);
}

public interface ICapriceFulfilledListener
{
    Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment fulfillment);
}

public interface ICapriceTurnLimit
{
    int MaxFulfillmentsPerTurn { get; }
}

public interface IRetainWhileEvading
{
}

public sealed class CapriceBagPower : AstolfoPower, IResourcePower
{
    public const int FullMask = 7;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int Mask => Math.Max(0, (int)Amount - 1);
}

public sealed class CurrentCapricePower : AstolfoPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public CommandType CapriceType => Caprices.FromBit((int)Amount);

    private string TypeKey => CapriceType switch
    {
        CommandType.Arts => "arts",
        CommandType.Buster => "buster",
        _ => "quick"
    };

    public override LocString Title => new("powers", $"{Id.Entry}.{TypeKey}Title");
    public override LocString Description => new("powers", $"{Id.Entry}.{TypeKey}Description");
    protected override string SmartDescriptionLocKey => $"{Id.Entry}.{TypeKey}SmartDescription";
}

public sealed class CapriceFulfilledThisTurnPower : AstolfoPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class QuickPlayedThisTurnPower : AstolfoPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class CapriceTurnStartedPower : AstolfoPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class LastCapricePower : AstolfoPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public CommandType CapriceType => Caprices.FromBit((int)Amount);
}

/// <summary>
/// Motor único de Capricho. La bolsa vive en Amount como mask+1, de modo que su estado vacío se
/// guarda. Sólo las Command normales pueden cumplirlo; el NP nunca consume el objetivo del turno.
/// </summary>
public sealed class CapriceControllerPower : AstolfoPower
{
    public const int QuickStars = 20;
    public const int ArtsNp = 20;
    public const int BusterDamageTotal = 6;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.IsDead) return;
        var firstTurn = Owner.GetPower<CapriceTurnStartedPower>() == null;
        if (firstTurn)
            await PowerCmd.Apply<CapriceTurnStartedPower>(
                context, Owner, 1m, Owner, null, silent: true);
        if (Owner.GetPower<CapriceFulfilledThisTurnPower>() is { } fulfilled)
            await PowerCmd.Remove(fulfilled);
        if (Owner.GetPower<QuickPlayedThisTurnPower>() is { } quicks)
            await PowerCmd.Remove(quicks);
        if (Owner.GetPower<AstolfoTurnUsagePower>() is { } usages)
            await PowerCmd.Remove(usages);

        // A Caprice chosen by a combat-start relic belongs to the first turn. From turn 2 onward,
        // an unfulfilled Caprice is discarded before drawing the next one.
        if (!firstTurn && Owner.GetPower<CurrentCapricePower>() != null)
            await Caprices.DiscardCurrent(context, Owner, null);
        if (Owner.GetPower<CurrentCapricePower>() == null)
            await Caprices.Draw(context, Owner, null);
        await MainFile.EnsureNpInCombat(context, Owner);
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner &&
            cardPlay.Card is AstolfoCommandCard { IsNoblePhantasm: false } command &&
            Caprices.Matches(Owner, command.CommandType) &&
            command.CommandType == CommandType.Buster)
        {
            command.CapriceDamageBonusTotal = BusterDamageTotal;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card is not AstolfoCommandCard { IsNoblePhantasm: false } command)
            return;

        try
        {
            if (command.CommandType == CommandType.Quick)
                await PowerCmd.Apply<QuickPlayedThisTurnPower>(
                    context, Owner, 1m, Owner, command, silent: true);
            if (!Caprices.Matches(Owner, command.CommandType)) return;
            var already = Caprices.FulfilledThisTurn(Owner);
            var limit = Math.Max(1, Listeners.Of<ICapriceTurnLimit>(Owner)
                .Select(rule => rule.MaxFulfillmentsPerTurn).DefaultIfEmpty(1).Max());
            if (already >= limit) return;

            Flash();
            switch (command.CommandType)
            {
                case CommandType.Quick:
                    await CritStars.Gain(context, Owner, QuickStars, command);
                    break;
                case CommandType.Arts:
                    await NpCharge.Gain(context, Owner, ArtsNp, command);
                    break;
                case CommandType.Buster:
                    break;
            }

            var current = Owner.GetPower<CurrentCapricePower>();
            if (current != null) await PowerCmd.Remove(current);
            await Caprices.SetLast(context, Owner, command.CommandType, command);
            await PowerCmd.Apply<CapriceFulfilledThisTurnPower>(
                context, Owner, 1m, Owner, command, silent: true);
            var count = already + 1;
            var result = new CapriceFulfillment(Owner, command, command.CommandType, count);
            await Listeners.ForEachListener<ICapriceFulfilledListener>(Owner,
                listener => listener.AfterCapriceFulfilled(context, result));

            if (count < limit)
                await Caprices.Draw(context, Owner, command);
        }
        finally
        {
            command.CapriceDamageBonusTotal = 0m;
        }
    }

    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Evasion.Of(Owner) <= 0 || Owner.Player == null)
            return Task.CompletedTask;
        foreach (var card in PileType.Hand.GetPile(Owner.Player).Cards.OfType<IRetainWhileEvading>())
            ((CardModel)card).GiveSingleTurnRetain();
        return Task.CompletedTask;
    }
}

public static class Caprices
{
    public static int Bit(CommandType type) => type switch
    {
        CommandType.Quick => 1,
        CommandType.Arts => 2,
        CommandType.Buster => 4,
        _ => 0
    };

    public static CommandType FromBit(int bit) => bit switch
    {
        1 => CommandType.Quick,
        2 => CommandType.Arts,
        4 => CommandType.Buster,
        _ => CommandType.Quick
    };

    public static CommandType? Current(Creature owner) =>
        owner.GetPower<CurrentCapricePower>()?.CapriceType;
    public static bool Matches(Creature owner, CommandType type) => Current(owner) == type;
    public static int FulfilledThisTurn(Creature owner) =>
        (int)owner.GetPowerAmount<CapriceFulfilledThisTurnPower>();
    public static int QuicksPlayedThisTurn(Creature owner) =>
        (int)owner.GetPowerAmount<QuickPlayedThisTurnPower>();

    public static IReadOnlyList<CommandType> Remaining(Creature owner)
    {
        var mask = owner.GetPower<CapriceBagPower>()?.Mask ?? CapriceBagPower.FullMask;
        if (mask == 0) mask = CapriceBagPower.FullMask;
        return new[] { CommandType.Quick, CommandType.Arts, CommandType.Buster }
            .Where(type => (mask & Bit(type)) != 0).ToList();
    }

    public static async Task EnsureInstalled(PlayerChoiceContext context, Creature owner)
    {
        if (owner.GetPower<CapriceBagPower>() == null)
            await PowerCmd.Apply<CapriceBagPower>(context, owner,
                CapriceBagPower.FullMask + 1m, owner, null, silent: true);
        if (owner.GetPower<CapriceControllerPower>() == null)
            await PowerCmd.Apply<CapriceControllerPower>(context, owner, 1m, owner, null, silent: true);
    }

    public static async Task Draw(PlayerChoiceContext context, Creature owner, CardModel? source)
    {
        await EnsureInstalled(context, owner);
        if (owner.GetPower<CurrentCapricePower>() != null) return;
        var bag = owner.GetPower<CapriceBagPower>()!;
        var mask = bag.Mask;
        if (mask == 0) mask = CapriceBagPower.FullMask;

        var options = new[] { 1, 2, 4 }.Where(bit => (mask & bit) != 0).ToList();
        if (bag.Mask == 0 && options.Count > 1 && owner.GetPower<LastCapricePower>() is { } last)
            options.Remove(Bit(last.CapriceType));
        var bit = options[owner.CombatState!.RunState.Rng.CombatCardGeneration.NextInt(options.Count)];
        await SetBag(context, owner, mask & ~bit, source);
        await PowerCmd.Apply<CurrentCapricePower>(context, owner, bit, owner, source);
        await Listeners.ForEachListener<ICapriceDrawListener>(owner,
            listener => listener.AfterCapriceDrawn(context, new CapriceDraw(owner, FromBit(bit))));
    }

    public static async Task Choose(
        PlayerChoiceContext context, Creature owner, CommandType type, CardModel? source,
        bool refill = false)
    {
        await EnsureInstalled(context, owner);
        if (owner.GetPower<CurrentCapricePower>() is { } current)
            await PowerCmd.Remove(current);
        var bag = owner.GetPower<CapriceBagPower>()!;
        var mask = refill ? CapriceBagPower.FullMask : bag.Mask;
        if (mask == 0) mask = CapriceBagPower.FullMask;
        await SetBag(context, owner, mask & ~Bit(type), source);
        await PowerCmd.Apply<CurrentCapricePower>(context, owner, Bit(type), owner, source);
        await Listeners.ForEachListener<ICapriceDrawListener>(owner,
            listener => listener.AfterCapriceDrawn(context, new CapriceDraw(owner, type)));
    }

    public static async Task DiscardCurrent(
        PlayerChoiceContext context, Creature owner, CardModel? source)
    {
        if (owner.GetPower<CurrentCapricePower>() is not { } current) return;
        var type = current.CapriceType;
        await PowerCmd.Remove(current);
        await SetLast(context, owner, type, source);
    }

    public static async Task Reroll(
        PlayerChoiceContext context, Creature owner, CardModel? source)
    {
        await DiscardCurrent(context, owner, source);
        await Draw(context, owner, source);
    }

    public static async Task SetLast(
        PlayerChoiceContext context, Creature owner, CommandType type, CardModel? source)
    {
        if (owner.GetPower<LastCapricePower>() is { } last)
            await PowerCmd.Remove(last);
        await PowerCmd.Apply<LastCapricePower>(context, owner, Bit(type), owner, source, silent: true);
    }

    private static async Task SetBag(
        PlayerChoiceContext context, Creature owner, int mask, CardModel? source)
    {
        var bag = owner.GetPower<CapriceBagPower>();
        if (bag == null)
        {
            await PowerCmd.Apply<CapriceBagPower>(context, owner, mask + 1m, owner, source, silent: true);
            return;
        }
        var desired = mask + 1m;
        if (bag.Amount != desired)
            await PowerCmd.ModifyAmount(context, bag, desired - bag.Amount, owner, source, silent: true);
    }
}
