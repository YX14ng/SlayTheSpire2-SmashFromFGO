using AstolfoRider.AstolfoRiderCode.Caprice;
using AstolfoRider.AstolfoRiderCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace AstolfoRider.AstolfoRiderCode.Powers;

public sealed class EvaporatedReasonDPlusPower : AstolfoPower, ICapriceTurnLimit
{
    public int MaxFulfillmentsPerTurn => 2;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
}

public sealed class PerfectImprovisationPower : AstolfoPower, ICapriceFulfilledListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment f)
    {
        if (f.Owner != Owner || Owner.Player == null) return;
        Flash();
        await CardPileCmd.Draw(context, 1, Owner.Player);
        await PowerCmd.Remove(this);
    }
    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class ThreeCapricesOneAdventurePower : AstolfoPower, ICapriceFulfilledListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    private int Mask => Math.Max(0, (int)Amount - 1);

    public async Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment f)
    {
        if (f.Owner != Owner) return;
        var mask = Mask | Caprices.Bit(f.Type);
        if (mask != CapriceBagPower.FullMask)
        {
            var desired = mask + 1m;
            if (desired != Amount)
                await PowerCmd.ModifyAmount(context, this, desired - Amount, Owner, f.Card, silent: true);
            return;
        }

        Flash();
        if (Amount != 1m)
            await PowerCmd.ModifyAmount(context, this, 1m - Amount, Owner, f.Card, silent: true);
        await Criticals.GrantReady(context, Owner, 1, f.Card);
        await PowerCmd.Apply<EnergyNextTurnPower>(context, Owner, 1m, Owner, f.Card);
    }
}

public sealed class NextNormalAttackBonusPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner &&
            cardPlay.Card is AstolfoCommandCard { IsNoblePhantasm: false, Type: CardType.Attack } command)
        {
            command.ExternalDamageBonusTotal += Amount;
            Flash();
        }
        return Task.CompletedTask;
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card is not AstolfoCommandCard { IsNoblePhantasm: false, Type: CardType.Attack } command) return;
        command.ExternalDamageBonusTotal = 0m;
        await PowerCmd.Remove(this);
    }
}

public sealed class FullSpeedGallopPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    private int TotalCap => Amount >= 2m ? 10 : 6;
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner &&
            cardPlay.Card is AstolfoCommandCard
            {
                IsNoblePhantasm: false,
                Type: CardType.Attack,
                CommandType: CommandType.Quick
            } command)
        {
            command.ExternalDamageBonusTotal += Math.Min(TotalCap, (int)Amount * command.DamagePortions);
            Flash();
        }
        return Task.CompletedTask;
    }
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is AstolfoCommandCard command) command.ExternalDamageBonusTotal = 0m;
        return Task.CompletedTask;
    }
}

public sealed class WorldReversePower : AstolfoPower, IEvasionConsumedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterEvasionConsumed(PlayerChoiceContext context, EvasionConsumed evasion)
    {
        if (AstolfoTurnUsages.WasUsed(Owner, AstolfoTurnUsage.WorldReverse) ||
            evasion.Owner != Owner) return;
        await AstolfoTurnUsages.Mark(context, Owner, AstolfoTurnUsage.WorldReverse, null);
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

public sealed class EnergyPenaltyNextTurnPower : AstolfoPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player) return;
        await PlayerCmd.LoseEnergy(Amount, player);
        await PowerCmd.Remove(this);
    }
}

public sealed class GoodDeedsWithoutThinkingPower : AstolfoPower, ICapriceFulfilledListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterCapriceFulfilled(PlayerChoiceContext context, CapriceFulfillment f)
    {
        if (f.Owner != Owner || f.NumberThisTurn != 1) return;
        var target = Owner.CombatState?.PlayerCreatures.Where(x => !x.IsDead)
            .OrderBy(x => x.MaxHp <= 0 ? 1m : x.CurrentHp / x.MaxHp).FirstOrDefault() ?? Owner;
        Flash();
        await CreatureCmd.GainBlock(target, Amount, ValueProp.Unpowered, null);
    }
}

public sealed class AdventureContinuesPower : AstolfoPower,
    ICriticalConsumedListener, IEvasionConsumedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (AstolfoTurnUsages.WasUsed(Owner, AstolfoTurnUsage.AdventureCritical) ||
            critical.Owner != Owner) return;
        await AstolfoTurnUsages.Mark(context, Owner, AstolfoTurnUsage.AdventureCritical, critical.Card);
        Flash();
        await NpCharge.Gain(context, Owner, 20, critical.Card);
    }
    public async Task AfterEvasionConsumed(PlayerChoiceContext context, EvasionConsumed evasion)
    {
        if (AstolfoTurnUsages.WasUsed(Owner, AstolfoTurnUsage.AdventureEvasion) ||
            evasion.Owner != Owner) return;
        await AstolfoTurnUsages.Mark(context, Owner, AstolfoTurnUsage.AdventureEvasion, null);
        Flash();
        await CritStars.Gain(context, Owner, 20, null);
    }
}

public sealed class ArgaliaKnockdownPower : AstolfoPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.IsDead || Owner.Monster == null ||
            Owner.IsStunned || !Owner.Monster.IntendsToAttack) return;
        Flash();
        var major = Owner.CombatState?.Encounter?.RoomType is RoomType.Elite or RoomType.Boss;
        if (major)
            await PowerCmd.Apply<WeakPower>(context, Owner, 3m, Applier, null);
        else
            await CreatureCmd.Stun(Owner);
        await PowerCmd.Decrement(this);
    }
}
