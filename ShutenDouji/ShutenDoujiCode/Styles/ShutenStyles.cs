using FGOCore.FGOCoreCode.Cleanse;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using ShutenDouji.ShutenDoujiCode.Powers;

namespace ShutenDouji.ShutenDoujiCode.Styles;

public enum ShutenStyle
{
    Assassin,
    Caster
}

public interface IShutenStyleCard
{
    ShutenStyle Style { get; }
    bool IsShutenNp { get; }
}

public readonly record struct StylePlay(
    Creature Owner,
    CardModel Card,
    ShutenStyle Style,
    bool FirstOfStyle,
    bool Crossed);

public interface IStylePlayedListener
{
    Task AfterStylePlayed(PlayerChoiceContext context, StylePlay play);
}

public interface IStyleCrossListener
{
    Task AfterStyleCrossed(PlayerChoiceContext context, StylePlay play);
}

/// <summary>
/// Historial persistible del turno. Amount codifica 1 + Assassin(1) + Caster(2), por lo que
/// guardar/cargar en mitad de combate no pierde el estado de Cruce.
/// </summary>
public sealed class StyleHistoryPower : ShutenPower, IResourcePower
{
    private const int Empty = 1;
    private const int AssassinBit = 1;
    private const int CasterBit = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;

    public bool HasPlayed(ShutenStyle style)
    {
        var bits = Math.Max(0, (int)Amount - Empty);
        var mask = style == ShutenStyle.Assassin ? AssassinBit : CasterBit;
        return (bits & mask) != 0;
    }

    public bool CrossedThisTurn => HasPlayed(ShutenStyle.Assassin) && HasPlayed(ShutenStyle.Caster);

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner ||
            cardPlay.Card is not IShutenStyleCard { IsShutenNp: false } styled)
        {
            return;
        }

        var first = !HasPlayed(styled.Style);
        var other = styled.Style == ShutenStyle.Assassin ? ShutenStyle.Caster : ShutenStyle.Assassin;
        var crossed = HasPlayed(other);

        if (first)
        {
            var bit = styled.Style == ShutenStyle.Assassin ? AssassinBit : CasterBit;
            await PowerCmd.ModifyAmount(context, this, bit, Owner, cardPlay.Card, silent: true);
        }

        var play = new StylePlay(Owner, cardPlay.Card, styled.Style, first, crossed);
        await Listeners.ForEachListener<IStylePlayedListener>(Owner,
            listener => listener.AfterStylePlayed(context, play));
        if (crossed)
        {
            await Listeners.ForEachListener<IStyleCrossListener>(Owner,
                listener => listener.AfterStyleCrossed(context, play));
        }
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Amount == Empty) return;
        await PowerCmd.ModifyAmount(context, this, Empty - Amount, Owner, null, silent: true);
    }

    public static async Task EnsureInstalled(PlayerChoiceContext context, Creature owner)
    {
        if (owner.GetPower<StyleHistoryPower>() == null)
        {
            await PowerCmd.Apply<StyleHistoryPower>(context, owner, Empty, owner, null, silent: true);
        }
    }
}

public static class StyleState
{
    public static bool HasCross(Creature owner, ShutenStyle currentStyle)
    {
        var other = currentStyle == ShutenStyle.Assassin ? ShutenStyle.Caster : ShutenStyle.Assassin;
        return owner.GetPower<StyleHistoryPower>()?.HasPlayed(other) == true;
    }

    public static bool CrossedThisTurn(Creature owner) =>
        owner.GetPower<StyleHistoryPower>()?.CrossedThisTurn == true;
}
