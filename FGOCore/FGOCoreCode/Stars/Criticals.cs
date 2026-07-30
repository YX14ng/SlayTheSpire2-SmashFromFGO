using FGOCore.FGOCoreCode.CardTypes;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Stars;

/// <summary>Recurso que pagó un crítico global.</summary>
public enum CriticalSource
{
    CritReady,
    Stars
}

/// <summary>Foto inmutable de un crítico ya pagado, emitida antes del primer golpe.</summary>
public readonly record struct CriticalHit(
    Creature Owner,
    CardPlay CardPlay,
    CriticalSource Source,
    int StarsSpent,
    decimal Multiplier)
{
    public CardModel Card => CardPlay.Card;

    public bool IsNoblePhantasm => Card is ICommandTyped { IsNoblePhantasm: true };
}

/// <summary>Reacciona una vez cuando una carta consume un crítico global.</summary>
public interface ICriticalConsumedListener
{
    Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical);
}

/// <summary>Regla opcional del dueño que puede cerrar temporalmente el gasto automático.</summary>
public interface ICriticalAccessRule
{
    bool CanSpendCritical(CardModel card);
}

/// <summary>Marcador explícito para el caso excepcional de un Noble Phantasm que sí puede criticar.</summary>
public interface INoblePhantasmCritical
{
}

/// <summary>API pública y única del sistema Critical v2.</summary>
public static class Criticals
{
    public const decimal DamageMultiplier = 1.5m;

    public static bool IsNoblePhantasm(CardModel card) =>
        card is ICommandTyped { IsNoblePhantasm: true };

    public static bool IsEligible(CardModel card) =>
        card.Type == CardType.Attack &&
        (!IsNoblePhantasm(card) || card is INoblePhantasmCritical);

    public static bool CanSpend(Creature owner, CardModel card)
    {
        if (!IsEligible(card)) return false;
        foreach (var rule in Listeners.Of<ICriticalAccessRule>(owner))
        {
            if (!rule.CanSpendCritical(card)) return false;
        }
        return true;
    }

    /// <summary>Predicción pura para hover/glow. No consume estrellas ni altera estado.</summary>
    public static bool WillCrit(Creature owner, CardModel card)
    {
        if (!CanSpend(owner, card)) return false;
        if (owner.GetPower<CriticalResolverPower>()?.IsActive(card) == true) return true;
        return owner.GetPowerAmount<CritReadyPower>() > 0m ||
               CritStars.CanPay(owner, CritStarsPower.CritCost);
    }

    /// <summary>Verdadero sólo durante la resolución de la carta que ya pagó el crítico.</summary>
    public static bool IsCritical(Creature owner, CardModel card) =>
        owner.GetPower<CriticalResolverPower>()?.IsActive(card) == true;

    public static bool IsCritical(CardPlay cardPlay) =>
        cardPlay.Card.Owner?.Creature is { } owner &&
        owner.GetPower<CriticalResolverPower>()?.IsActive(cardPlay) == true;

    public static Task EnsureInstalled(Creature owner) =>
        EnsureInstalled(new BlockingPlayerChoiceContext(), owner);

    public static async Task EnsureInstalled(PlayerChoiceContext context, Creature owner)
    {
        if (owner.GetPower<CriticalResolverPower>() == null)
        {
            await PowerCmd.Apply<CriticalResolverPower>(context, owner, 1m, owner, null, silent: true);
        }
    }

    /// <summary>Concede Crítico Listo respetando su máximo global de tres cargas.</summary>
    public static async Task GrantReady(
        PlayerChoiceContext context, Creature owner, int amount, CardModel? source = null)
    {
        if (amount <= 0) return;
        await EnsureInstalled(context, owner);
        var room = CritReadyPower.MaxStacks - (int)owner.GetPowerAmount<CritReadyPower>();
        if (room <= 0) return;
        await PowerCmd.Apply<CritReadyPower>(context, owner, Math.Min(amount, room), owner, source);
    }
}

/// <summary>
/// Motor oculto que reserva el recurso antes de resolver la carta y aplica ×1.5 a todos sus golpes.
/// El estado se ata al CardPlay para que Repetir y los previews no gasten ni hereden críticos.
/// </summary>
public sealed class CriticalResolverPower : FGOCorePower
{
    private CardPlay? _activePlay;
    private CardModel? _activeCard;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldScaleInMultiplayer => false;

    internal bool IsActive(CardModel card) => _activeCard == card;

    internal bool IsActive(CardPlay cardPlay) => _activePlay == cardPlay;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        ClearActive();

        var card = cardPlay.Card;
        if (card.Owner?.Creature != Owner || !Criticals.CanSpend(Owner, card)) return;

        CriticalSource source;
        var starsSpent = 0;

        // Se marca antes de tocar recursos: los hooks de cambio de power ya ven la carta correcta.
        _activePlay = cardPlay;
        _activeCard = card;

        // El hook vanilla no entrega el contexto original. Un único contexto bloqueante conserva el
        // orden determinista y permite reaccionar antes del primer golpe sin abrir elecciones.
        var context = new BlockingPlayerChoiceContext();
        if (Owner.GetPower<CritReadyPower>() is { Amount: > 0 } ready)
        {
            source = CriticalSource.CritReady;
            await PowerCmd.ModifyAmount(context, ready, -1m, Owner, card);
        }
        else if (await CritStars.Spend(context, Owner, CritStarsPower.CritCost, card))
        {
            source = CriticalSource.Stars;
            starsSpent = CritStarsPower.CritCost;
        }
        else
        {
            ClearActive();
            return;
        }

        var critical = new CriticalHit(Owner, cardPlay, source, starsSpent, Criticals.DamageMultiplier);
        await Listeners.ForEachListener<ICriticalConsumedListener>(
            Owner, listener => listener.AfterCriticalConsumed(context, critical));
    }

    public override decimal ModifyDamageMultiplicativeFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 1m;

        if (_activeCard == cardSource &&
            (_activePlay == null || cardPlay == null || _activePlay == cardPlay))
        {
            return Criticals.DamageMultiplier;
        }

        // Fuera de una resolución real, el juego usa este hook para el preview. La consulta es pura.
        return _activeCard == null && Criticals.WillCrit(Owner, cardSource)
            ? Criticals.DamageMultiplier
            : 1m;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activePlay == cardPlay) ClearActive();
        return Task.CompletedTask;
    }

    private void ClearActive()
    {
        _activePlay = null;
        _activeCard = null;
    }
}
