using FGOCore.FGOCoreCode.Cleanse;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;
using SakeBank = ShutenDouji.ShutenDoujiCode.Sake.Sake;

namespace ShutenDouji.ShutenDoujiCode.Powers;

public static class PoisonDamageRules
{
    public static bool IsPoisonTick(
        Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource) =>
        dealer == null && cardSource == null && result.UnblockedDamage > 0 &&
        props.HasFlag(ValueProp.Unblockable) && props.HasFlag(ValueProp.Unpowered) &&
        target.HasPower<PoisonPower>();
}

public abstract class HiddenTurnMarkerPower : ShutenPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class AssassinPlayedThisTurnPower : HiddenTurnMarkerPower;
public sealed class FruityAromaExUsedPower : HiddenTurnMarkerPower;
public sealed class OrochiBloodUsedPower : HiddenTurnMarkerPower;
public sealed class GohoOniUsedPower : HiddenTurnMarkerPower;
public sealed class FullPowerMagicalGirlUsedPower : HiddenTurnMarkerPower;
public sealed class TwoOutfitsUsedPower : HiddenTurnMarkerPower;
public sealed class MountOoeKanzashiUsedPower : HiddenTurnMarkerPower;
public sealed class HakuBellUsedPower : HiddenTurnMarkerPower;
public sealed class PoisonedCupUsedPower : HiddenTurnMarkerPower;
public sealed class RedDragonUlnaUsedPower : HiddenTurnMarkerPower;
public sealed class KuzuryuFragmentUsedPower : HiddenTurnMarkerPower;
public sealed class AntiquitiesTreasureChosenPower : HiddenTurnMarkerPower;
public sealed class ArtsBusterReinforcementArtsUsedPower : HiddenTurnMarkerPower;
public sealed class ArtsBusterReinforcementBusterUsedPower : HiddenTurnMarkerPower;
public sealed class DivineProtectionBreakerUsedPower : HiddenTurnMarkerPower;

public sealed class MountOoeBanquetPower : ShutenPower, IStylePlayedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterStylePlayed(PlayerChoiceContext context, StylePlay play)
    {
        if (play.Owner == Owner && play.Style == ShutenStyle.Assassin &&
            play.Card is IShutenStyleCard { IsShutenNp: false } &&
            !Owner.HasPower<AssassinPlayedThisTurnPower>())
            await PowerCmd.Apply<AssassinPlayedThisTurnPower>(context, Owner, 1m, Owner, play.Card, silent: true);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        if (Owner.GetPower<AssassinPlayedThisTurnPower>() is { } marker) await PowerCmd.Remove(marker);
        Flash();
        await SakeBank.Gain(new BlockingPlayerChoiceContext(), Owner, 20, null);
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || !Owner.HasPower<AssassinPlayedThisTurnPower>()) return;
        Flash();
        foreach (var enemy in Owner.CombatState!.HittableEnemies)
            await PowerCmd.Apply<PoisonPower>(context, enemy, Amount, Owner, null);
    }
}

public sealed class FruityAromaExPower : ShutenPower
{
    private int NpGain => Amount >= 2 ? 20 : 10;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override int DisplayAmount => NpGain;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.HasPower<FruityAromaExUsedPower>() || amount <= 0m || applier != Owner ||
            cardSource?.Owner?.Creature != Owner || power.Owner.Side == Owner.Side ||
            power.TypeForCurrentAmount != PowerType.Debuff || !power.IsVisible || power is IResourcePower) return;

        await PowerCmd.Apply<FruityAromaExUsedPower>(context, Owner, 1m, Owner, cardSource, silent: true);
        Flash();
        await SakeBank.Gain(context, Owner, 20, cardSource);
        await NpCharge.Gain(context, Owner, NpGain, cardSource);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Owner.GetPower<FruityAromaExUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class OrochiBloodPower : ShutenPower
{
    private int NpGain => Amount >= 2 ? 20 : 10;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override int DisplayAmount => NpGain;

    public override async Task AfterDamageGiven(
        PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (Owner.HasPower<OrochiBloodUsedPower>() ||
            !PoisonDamageRules.IsPoisonTick(dealer, result, props, target, cardSource) ||
            target.Side == Owner.Side) return;

        await PowerCmd.Apply<OrochiBloodUsedPower>(context, Owner, 1m, Owner, null, silent: true);
        Flash();
        await NpCharge.Gain(context, Owner, NpGain, null);
        await CritStars.Gain(context, Owner, 10, null);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Owner.GetPower<OrochiBloodUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class HakuWhiteFamiliarPower : ShutenPower
{
    private int Block => Amount;
    private int PaidBonus => Amount >= 8 ? 5 : 4;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        var context = new BlockingPlayerChoiceContext();
        var paid = SakeBank.CanSpend(Owner, 10);
        Flash();
        await CreatureCmd.GainBlock(Owner, Block + (paid ? PaidBonus : 0), ValueProp.Unpowered, null);
        if (paid && await SakeBank.Spend(context, Owner, 10, null))
            await NpCharge.Gain(context, Owner, 10, null);
    }
}

public sealed class GohoOniPower : ShutenPower
{
    private int SakeGain => Amount >= 2 ? 20 : 10;
    private int BlockGain => Amount >= 2 ? 5 : 0;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override int DisplayAmount => SakeGain;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (Owner.HasPower<GohoOniUsedPower>() || amount <= 0m || power.Owner.Side == Owner.Side ||
            !Cleanse.IsOffensiveBuff(power)) return;

        await PowerCmd.Apply<GohoOniUsedPower>(context, Owner, 1m, Owner, cardSource, silent: true);
        Flash();
        await PowerCmd.Remove(power);
        await SakeBank.Gain(context, Owner, SakeGain, cardSource);
        if (BlockGain > 0) await CreatureCmd.GainBlock(Owner, BlockGain, ValueProp.Unpowered, null);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Owner.GetPower<GohoOniUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class FullPowerMagicalGirlPower : ShutenPower
{
    private CardPlay? _activePlay;
    private decimal _remaining;
    private int PerHit => Amount >= 2 ? 6 : 5;
    private int Cap => Amount >= 2 ? 24 : 20;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override int DisplayAmount => PerHit;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Owner.HasPower<FullPowerMagicalGirlUsedPower>() || cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card.Type != CardType.Attack ||
            cardPlay.Card is not IShutenStyleCard { Style: ShutenStyle.Caster, IsShutenNp: false } ||
            !SakeBank.CanSpend(Owner, 20)) return;

        var context = new BlockingPlayerChoiceContext();
        if (!await SakeBank.Spend(context, Owner, 20, cardPlay.Card)) return;
        await PowerCmd.Apply<FullPowerMagicalGirlUsedPower>(context, Owner, 1m, Owner, cardPlay.Card, silent: true);
        _activePlay = cardPlay;
        _remaining = Cap;
        Flash();
    }

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (_activePlay == null || dealer != Owner || cardSource != _activePlay.Card ||
            !props.IsPoweredAttack() || _remaining <= 0m) return 0m;
        return Math.Min(PerHit, _remaining);
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (_activePlay != null && dealer == Owner && cardSource == _activePlay.Card && props.IsPoweredAttack())
        {
            _remaining = Math.Max(0m, _remaining - Math.Min(PerHit, _remaining));
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activePlay == cardPlay)
        {
            _activePlay = null;
            _remaining = 0m;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Owner.GetPower<FullPowerMagicalGirlUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class OneSaintGraphTwoOutfitsPower : ShutenPower, IStyleCrossListener
{
    private int NpGain => Amount >= 2 ? 20 : 10;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override int DisplayAmount => NpGain;

    public async Task AfterStyleCrossed(PlayerChoiceContext context, StylePlay play)
    {
        if (play.Owner != Owner || Owner.HasPower<TwoOutfitsUsedPower>()) return;
        await PowerCmd.Apply<TwoOutfitsUsedPower>(context, Owner, 1m, Owner, play.Card, silent: true);
        Flash();
        await CardPileCmd.Draw(context, 1, Owner.Player!);
        await NpCharge.Gain(context, Owner, NpGain, play.Card);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Owner.GetPower<TwoOutfitsUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class BanquetNeverEndsPower : ShutenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || !StyleState.CrossedThisTurn(Owner) || Owner.Player == null) return;
        if (!await SakeBank.Spend(context, Owner, 20, null)) return;
        Flash();
        await PowerCmd.Apply<EnergyNextTurnPower>(context, Owner, 1m, Owner, null);
    }
}

public sealed class OniHeadGutsPower : GutsPower
{
    protected override async Task OnTriggered(PlayerChoiceContext context)
    {
        await SakeBank.Gain(context, Owner, 50, null);
    }
}

public sealed class KuzuryuFragmentAttackPower : ShutenPower
{
    private const int TotalCap = 12;
    private CardPlay? _activePlay;
    private decimal _remaining;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner && cardPlay.Card.Type == CardType.Attack &&
            cardPlay.Card is not ICommandTyped { IsNoblePhantasm: true })
        {
            _activePlay = cardPlay;
            _remaining = TotalCap;
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (_activePlay == null || dealer != Owner || cardSource != _activePlay.Card ||
            !props.IsPoweredAttack() || _remaining <= 0m) return 0m;
        return Math.Min(Amount, _remaining);
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (_activePlay != null && dealer == Owner && cardSource == _activePlay.Card && props.IsPoweredAttack())
        {
            _remaining = Math.Max(0m, _remaining - Math.Min(Amount, _remaining));
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activePlay == cardPlay) await PowerCmd.Remove(this);
    }
}
