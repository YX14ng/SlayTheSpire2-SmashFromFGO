using FGOCore.FGOCoreCode.Cleanse;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ShutenDouji.ShutenDoujiCode.Cards.Uncommon;
using ShutenDouji.ShutenDoujiCode.Sake;
using ShutenDouji.ShutenDoujiCode.Styles;

namespace ShutenDouji.ShutenDoujiCode.Powers;

public sealed class InvitationToPerditionPower : TemporaryStrengthPower, ICustomModel
{
    public override AbstractModel OriginModel => ModelDb.Card<InvitationToPerdition>();
    protected override bool IsPositive => false;
}

public sealed class PresenceConcealmentUsedPower : ShutenPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class PresenceConcealmentPower : ShutenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card is not ICommandTyped { CommandType: CommandType.Quick, IsNoblePhantasm: false } ||
            Owner.HasPower<PresenceConcealmentUsedPower>()) return;

        await PowerCmd.Apply<PresenceConcealmentUsedPower>(context, Owner, 1m, Owner, cardPlay.Card, silent: true);
        Flash();
        await CritStars.Gain(context, Owner, Amount, cardPlay.Card);
        await Sake.Sake.Gain(context, Owner, 10, cardPlay.Card);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Owner.GetPower<PresenceConcealmentUsedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class SakeTurnProgressPower : ShutenPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class PoisonedTablePower : ShutenPower, ISakeGainedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public Task AfterSakeGained(PlayerChoiceContext context, SakeChange change)
    {
        if (change.Owner != Owner || change.Amount <= 0) return Task.CompletedTask;
        return PowerCmd.Apply<SakeTurnProgressPower>(context, Owner, change.Amount, Owner, change.Source, silent: true);
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        var progress = Owner.GetPower<SakeTurnProgressPower>();
        if (progress?.Amount >= 30)
        {
            Flash();
            foreach (var enemy in Owner.CombatState!.HittableEnemies)
                await PowerCmd.Apply<PoisonPower>(context, enemy, Amount, Owner, null);
        }
        if (progress != null) await PowerCmd.Remove(progress);
    }
}

public sealed class NextPoisonAmplifierPower : ShutenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyPowerAmountGivenAdditive(
        PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource) =>
        giver == Owner && power is PoisonPower && amount > 0m ? Amount : 0m;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is PoisonPower && amount > 0m && applier == Owner)
        {
            Flash();
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class DragonGodDaughterUsesPower : ShutenPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class DragonGodDaughterPower : ShutenPower
{
    private CardPlay? _activePlay;
    private bool _triggeredForAttack;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner &&
            cardPlay.Card.Type == CardType.Attack &&
            cardPlay.Card is IShutenStyleCard { Style: ShutenStyle.Assassin, IsShutenNp: false })
        {
            _activePlay = cardPlay;
            _triggeredForAttack = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (_activePlay == null || _triggeredForAttack || dealer != Owner ||
            _activePlay.Card != cardSource || !props.IsPoweredAttack() || result.UnblockedDamage <= 0 ||
            Owner.GetPowerAmount<DragonGodDaughterUsesPower>() >= 3) return;

        _triggeredForAttack = true;
        await PowerCmd.Apply<DragonGodDaughterUsesPower>(context, Owner, 1m, Owner, cardSource, silent: true);
        Flash();
        await PowerCmd.Apply<PoisonPower>(context, target, Amount, Owner, cardSource);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activePlay == cardPlay)
        {
            _activePlay = null;
            _triggeredForAttack = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Owner.GetPower<DragonGodDaughterUsesPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}

public sealed class ArtsBusterReinforcementPower : ShutenPower
{
    private CardPlay? _activeBuster;
    private bool _busterHit;

    private int Rank => Math.Clamp(Amount, 1, 2);
    private int ArtsNp => Rank == 1 ? 10 : 20;
    private int BusterDamage => Rank == 1 ? 3 : 4;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override int DisplayAmount => BusterDamage;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!Owner.HasPower<ArtsBusterReinforcementBusterUsedPower>() &&
            cardPlay.Card.Owner?.Creature == Owner &&
            cardPlay.Card is ICommandTyped { CommandType: CommandType.Buster, IsNoblePhantasm: false })
        {
            _activeBuster = cardPlay;
            _busterHit = false;
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (_activeBuster == null || _busterHit || dealer != Owner ||
            cardSource != _activeBuster.Card || !props.IsPoweredAttack()) return 0m;
        return BusterDamage;
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (_activeBuster != null && !_busterHit && dealer == Owner &&
            cardSource == _activeBuster.Card && props.IsPoweredAttack()) _busterHit = true;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || cardPlay.Card is not ICommandTyped typed || typed.IsNoblePhantasm)
            return;

        if (!Owner.HasPower<ArtsBusterReinforcementArtsUsedPower>() && typed.CommandType == CommandType.Arts)
        {
            await PowerCmd.Apply<ArtsBusterReinforcementArtsUsedPower>(
                context, Owner, 1m, Owner, cardPlay.Card, silent: true);
            Flash();
            await NpCharge.Gain(context, Owner, ArtsNp, cardPlay.Card);
        }
        if (_activeBuster == cardPlay)
        {
            await PowerCmd.Apply<ArtsBusterReinforcementBusterUsedPower>(
                context, Owner, 1m, Owner, cardPlay.Card, silent: true);
            _activeBuster = null;
            _busterHit = false;
        }
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            _activeBuster = null;
            _busterHit = false;
            if (Owner.GetPower<ArtsBusterReinforcementArtsUsedPower>() is { } artsMarker)
                await PowerCmd.Remove(artsMarker);
            if (Owner.GetPower<ArtsBusterReinforcementBusterUsedPower>() is { } busterMarker)
                await PowerCmd.Remove(busterMarker);
        }
    }
}

public sealed class DivineProtectionBreakerPower : ShutenPower
{
    private CardPlay? _activePlay;
    private bool _usedHit;

    private int Rank => Math.Clamp(Amount, 1, 2);
    private int DamageBonus => Rank == 1 ? 5 : 7;
    private int NpBonus => Rank == 1 ? 10 : 20;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    public override int DisplayAmount => DamageBonus;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!Owner.HasPower<DivineProtectionBreakerUsedPower>() &&
            cardPlay.Card.Owner?.Creature == Owner &&
            cardPlay.Target != null && cardPlay.Card.Type == CardType.Attack &&
            cardPlay.Card is IShutenStyleCard { Style: ShutenStyle.Caster, IsShutenNp: false } &&
            cardPlay.Target.Powers.Any(power => power.TypeForCurrentAmount == PowerType.Debuff && power is not IResourcePower))
        {
            _activePlay = cardPlay;
            _usedHit = false;
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (_activePlay == null || _usedHit || dealer != Owner ||
            cardSource != _activePlay.Card || !props.IsPoweredAttack()) return 0m;
        return DamageBonus;
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext context, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (_activePlay != null && !_usedHit && dealer == Owner &&
            cardSource == _activePlay.Card && props.IsPoweredAttack()) _usedHit = true;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activePlay != cardPlay) return;
        _activePlay = null;
        _usedHit = false;
        await PowerCmd.Apply<DivineProtectionBreakerUsedPower>(
            context, Owner, 1m, Owner, cardPlay.Card, silent: true);
        Flash();
        await NpCharge.Gain(context, Owner, NpBonus, cardPlay.Card);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            _activePlay = null;
            _usedHit = false;
            if (Owner.GetPower<DivineProtectionBreakerUsedPower>() is { } marker)
                await PowerCmd.Remove(marker);
        }
    }
}
