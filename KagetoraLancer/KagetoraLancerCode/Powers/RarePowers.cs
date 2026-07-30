using KagetoraLancer.KagetoraLancerCode.Doctrine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

public sealed class WhiteFlamePower : KagetoraPower, IDoctrineAdvanceListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        await CritStars.Gain(Owner, 10, null);
    }

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.WhiteFlame) ||
            !result.Advanced || result.Attempted != Precept.Heaven) return;
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.WhiteFlame, result.CardPlay.Card);
        Flash();
        await NpCharge.Gain(context, Owner, (int)Amount, result.CardPlay.Card);
    }
}

public sealed class EightFormationsPower : KagetoraPower, IDoctrineFailureOverride
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public bool CanOverrideDoctrineFailure(CardPlay cardPlay, Precept attempted) =>
        !KagetoraUsages.WasUsed(Owner, KagetoraUsage.EightFormations) &&
        cardPlay.Card.Owner?.Creature == Owner && attempted != Precept.None;

    public async Task AfterOverridingDoctrineFailure(PlayerChoiceContext context, CardPlay cardPlay, Precept attempted)
    {
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.EightFormations, cardPlay.Card);
        Flash();
    }
}

public sealed class ForcedDoctrineAdvancePower : KagetoraPower,
    IDoctrineFailureOverride, IDoctrineAdvanceListener
{
    private CardModel? _card;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int DoctrineOverridePriority => 100;

    public void Arm(CardModel card) => _card = card;
    public bool CanOverrideDoctrineFailure(CardPlay cardPlay, Precept attempted) =>
        _card == cardPlay.Card && attempted != Precept.None;

    public async Task AfterOverridingDoctrineFailure(PlayerChoiceContext context, CardPlay cardPlay, Precept attempted) =>
        await PowerCmd.Remove(this);

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (_card == result.CardPlay.Card && Owner.HasPower<ForcedDoctrineAdvancePower>())
            await PowerCmd.Remove(this);
    }
}

public sealed class TreasureInHeartPower : KagetoraPower, IDoctrineAdvanceListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (!result.Advanced || result.Attempted != Precept.Chest) return;
        Flash();
        await PowerCmd.Apply<TreasureWindowPower>(context, Owner, Amount, Owner, result.CardPlay.Card);
    }
}

public sealed class TreasureWindowPower : KagetoraPower
{
    private bool _prevented;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target,
        decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (_prevented || target != Owner || applier == null || applier.Side == target.Side ||
            canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff || !canonicalPower.IsVisible) return false;
        modifiedAmount = 0m;
        return true;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (_prevented) return;
        _prevented = true;
        Flash();
        await NpCharge.Gain(new BlockingPlayerChoiceContext(), Owner, (int)Amount, null);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class FieldJudgePower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(context, power, amount, applier, cardSource);
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.FieldJudge) ||
            amount <= 0 || power.Owner.Side == Owner.Side ||
            power.GetTypeForAmount(amount) != PowerType.Buff || !power.IsVisible) return;
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.FieldJudge, cardSource);
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        await NpCharge.Gain(context, Owner, Amount >= 12m ? 20 : 10, cardSource);
    }
}

public sealed class VictoryIsInTheFeetPower : KagetoraPower, ICriticalConsumedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.VictoryInTheFeet) ||
            critical.Owner != Owner) return;
        await KagetoraUsages.Mark(
            context, Owner, KagetoraUsage.VictoryInTheFeet, critical.Card);
        Flash();
        await CritStars.Gain(context, Owner, 20, critical.Card);
        if (Amount >= 2m) await NpCharge.Gain(context, Owner, 10, critical.Card);
    }
}

public sealed class BishamontenManifestationPower : KagetoraPower, IDoctrineCycleListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (Amount <= 0) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(context, Owner, 1m, Owner, result.CardPlay.Card);
        await PowerCmd.Decrement(this);
    }
}
