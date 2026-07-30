using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Doctrine;

public enum Precept
{
    None = 0,
    Heaven = 1,
    Chest = 2,
    Feet = 4
}

public interface IPreceptCard
{
    Precept Precept { get; }
}

public readonly record struct DoctrineAdvance(
    CardPlay CardPlay,
    Precept Attempted,
    bool Advanced,
    int BeforeMask,
    int AfterMask,
    bool CycleCompleted,
    int AdvancesThisTurn);

public interface IDoctrineAdvanceListener
{
    Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result);
}

public interface IDoctrineCycleListener
{
    Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result);
}

public interface IDoctrineFailureOverride
{
    int DoctrineOverridePriority => 0;
    bool CanOverrideDoctrineFailure(CardPlay cardPlay, Precept attempted);
    Task AfterOverridingDoctrineFailure(PlayerChoiceContext context, CardPlay cardPlay, Precept attempted);
}

/// <summary>
/// Motor único de la Doctrina. Amount guarda mask+1 para que el estado vacío siga existiendo y
/// sobreviva guardado/carga. Kagetora exige Heaven→Chest→Feet; Kenshin acepta cualquier orden sin
/// repetir. Una carta sólo puede producir un intento y nunca borra progreso al fallar.
/// </summary>
public sealed class DoctrinePower : KagetoraPower
{
    public const int MaxAdvancesPerTurn = 3;
    public const int HeavenNp = 10;
    public const int ChestBlock = 4;
    public const int FeetStars = 20;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public int ProgressMask => Math.Max(0, (int)Amount - 1);
    public int AdvancesThisTurn => Owner.GetPower<DoctrineTurnStatePower>()?.Advances ?? 0;
    public int AdvancedMaskThisTurn => Owner.GetPower<DoctrineTurnStatePower>()?.AdvancedMask ?? 0;
    public bool IsKenshin => Owner.HasPower<KenshinFormPower>();

    public static async Task EnsureInstalled(Creature owner)
    {
        if (!owner.HasPower<DoctrinePower>())
        {
            await PowerCmd.Apply<DoctrinePower>(
                new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
        }
    }

    public bool WouldAdvance(Precept precept)
    {
        if (precept == Precept.None || AdvancesThisTurn >= MaxAdvancesPerTurn) return false;
        var mask = ProgressMask;
        if (IsKenshin) return (mask & (int)precept) == 0;

        var expected = (mask & (int)Precept.Heaven) == 0 ? Precept.Heaven
            : (mask & (int)Precept.Chest) == 0 ? Precept.Chest
            : (mask & (int)Precept.Feet) == 0 ? Precept.Feet
            : Precept.None;
        return precept == expected;
    }

    /// <summary>Predice qué etiqueta podría avanzar después de resolver la carta actual.</summary>
    public bool WouldAdvanceAfter(Precept current, Precept next)
    {
        if (next == Precept.None || AdvancesThisTurn >= MaxAdvancesPerTurn) return false;
        var mask = ProgressMask;
        var advances = AdvancesThisTurn;
        if (WouldAdvance(current))
        {
            mask |= (int)current;
            advances++;
            if (mask == 7) mask = 0;
        }
        if (advances >= MaxAdvancesPerTurn) return false;
        if (IsKenshin) return (mask & (int)next) == 0;
        var expected = (mask & (int)Precept.Heaven) == 0 ? Precept.Heaven
            : (mask & (int)Precept.Chest) == 0 ? Precept.Chest
            : (mask & (int)Precept.Feet) == 0 ? Precept.Feet
            : Precept.None;
        return next == expected;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        await MainFile.EnsureNpInCombat(Owner);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || cardPlay.Card is not IPreceptCard tagged) return;

        var precept = tagged.Precept;
        var before = ProgressMask;
        var advanced = WouldAdvance(precept);
        IDoctrineFailureOverride? overrideRule = null;
        if (!advanced && precept != Precept.None && AdvancesThisTurn < MaxAdvancesPerTurn)
        {
            overrideRule = Listeners.Of<IDoctrineFailureOverride>(Owner)
                .OrderByDescending(rule => rule.DoctrineOverridePriority)
                .FirstOrDefault(rule => rule.CanOverrideDoctrineFailure(cardPlay, precept));
            advanced = overrideRule != null;
        }
        if (!advanced)
        {
            var miss = new DoctrineAdvance(cardPlay, precept, false, before, before, false, AdvancesThisTurn);
            await Listeners.ForEachListener<IDoctrineAdvanceListener>(
                Owner, listener => listener.AfterDoctrineAdvance(context, miss));
            return;
        }

        // La recompensa se concede antes de cambiar el estado y de emitir eventos.
        if (overrideRule != null)
            await overrideRule.AfterOverridingDoctrineFailure(context, cardPlay, precept);
        await GrantInnateReward(context, precept, cardPlay.Card);

        var advancesThisTurn = AdvancesThisTurn + 1;
        var advancedMaskThisTurn = AdvancedMaskThisTurn | (int)precept;
        await DoctrineTurnState.Set(
            context, Owner, advancesThisTurn, advancedMaskThisTurn, cardPlay.Card);
        var advanceBit = (int)precept;
        if (overrideRule != null && (before & advanceBit) != 0)
        {
            advanceBit = new[] { 1, 2, 4 }.FirstOrDefault(bit => (before & bit) == 0);
        }
        var after = before | advanceBit;
        var completed = after == (int)(Precept.Heaven | Precept.Chest | Precept.Feet);
        await SetProgress(context, after, cardPlay.Card);

        var result = new DoctrineAdvance(
            cardPlay, precept, true, before, after, completed, advancesThisTurn);
        await Listeners.ForEachListener<IDoctrineAdvanceListener>(
            Owner, listener => listener.AfterDoctrineAdvance(context, result));

        if (!completed) return;

        // Vaciar antes del evento permite que sus listeners consulten el siguiente estado correcto.
        await SetProgress(context, 0, cardPlay.Card);
        await Listeners.ForEachListener<IDoctrineCycleListener>(
            Owner, listener => listener.AfterDoctrineCycle(context, result));
    }

    private async Task SetProgress(PlayerChoiceContext context, int mask, CardModel source)
    {
        var desired = mask + 1m;
        var delta = desired - Amount;
        if (delta != 0m)
        {
            await PowerCmd.ModifyAmount(context, this, delta, Owner, source);
        }
    }

    private async Task GrantInnateReward(
        PlayerChoiceContext context, Precept precept, CardModel source)
    {
        Flash();
        switch (precept)
        {
            case Precept.Heaven:
                await NpCharge.Gain(context, Owner, HeavenNp, source);
                break;
            case Precept.Chest:
                await CreatureCmd.GainBlock(Owner, ChestBlock, ValueProp.Unpowered, null);
                break;
            case Precept.Feet:
                await CritStars.Gain(context, Owner, FeetStars, source);
                break;
        }
    }
}
