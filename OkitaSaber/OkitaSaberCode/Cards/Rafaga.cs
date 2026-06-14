using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards;

/// <summary>
/// Un power del owner que ALTERA cómo se paga el segundo coste de una *Ráfaga. Lo implementan
/// la forma «Flor del Bakumatsu» (gratis) y «Hasta el Final» (pagás HP en vez de Aliento).
/// </summary>
public interface IRafagaCostModifier
{
    /// <summary>Si true, las Ráfagas NO consumen Aliento mientras este power esté activo.</summary>
    bool WaivesBreathCost { get; }

    /// <summary>HP a pagar por cada punto de Aliento que se habría pagado (0 = no paga HP).</summary>
    int HpPerBreathPoint { get; }
}

/// <summary>
/// Helper de la keyword *RÁFAGA. Centraliza el pago del segundo coste (Aliento) para que los
/// modificadores (Flor del Bakumatsu, Hasta el Final, Paso Constante) apliquen en un solo lugar.
/// Llamá <see cref="Pay"/> al PRINCIPIO del OnPlay de toda carta <see cref="IRafagaCard"/>.
/// </summary>
public static class Rafaga
{
    /// <summary>¿Puede jugarse esta Ráfaga? (Aliento suficiente, o un override que exime el coste.)</summary>
    public static bool IsPlayable(Creature creature, int rafagaCost)
    {
        if (Waived(creature)) return true;
        return Aliento.CanPay(creature, rafagaCost);
    }

    /// <summary>Glow dorado de las Ráfagas: cuando el Aliento alcanza (o un override las exime).</summary>
    public static bool ShouldGlow(Creature creature, int rafagaCost) => IsPlayable(creature, rafagaCost);

    /// <summary>
    /// Paga el segundo coste de la Ráfaga. Con un override que exime el Aliento (Flor del
    /// Bakumatsu / Hasta el Final) no gasta Aliento; Hasta el Final cobra HP en su lugar.
    /// «Paso Constante» (<see cref="IFirstRafagaRefund"/>) reembolsa Aliento en la 1ª Ráfaga del turno.
    /// </summary>
    public static async Task Pay(PlayerChoiceContext choiceContext, Creature creature, int rafagaCost, CardModel? source)
    {
        var mod = Modifier(creature);
        if (mod != null && mod.WaivesBreathCost)
        {
            if (mod.HpPerBreathPoint > 0)
            {
                var hp = rafagaCost * mod.HpPerBreathPoint;
                await CreatureCmd.Damage(choiceContext, creature, hp,
                    ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            }
            return;
        }

        await Aliento.Spend(creature, rafagaCost, source);
        await TryRefundFirst(creature, source);
    }

    private static bool Waived(Creature creature)
    {
        var mod = Modifier(creature);
        return mod != null && mod.WaivesBreathCost;
    }

    private static IRafagaCostModifier? Modifier(Creature creature)
    {
        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is IRafagaCostModifier mod) return mod;
        }
        return null;
    }

    // «Paso Constante»: la primera Ráfaga de cada turno reembolsa 1 Aliento (cap del descuento).
    private static async Task TryRefundFirst(Creature creature, CardModel? source)
    {
        foreach (var power in creature.GetPowerInstances<PowerModel>())
        {
            if (power is IFirstRafagaRefund refund && refund.TryConsumeRefund())
            {
                await Aliento.Gain(creature, refund.RefundAmount, source);
                if (refund.RefundStars > 0) await CritStars.Gain(creature, refund.RefundStars, source);
                return;
            }
        }
    }
}

/// <summary>«Paso Constante»: la 1ª *Ráfaga del turno reembolsa Aliento (y opcionalmente ★).</summary>
public interface IFirstRafagaRefund
{
    int RefundAmount { get; }
    int RefundStars { get; }

    /// <summary>Devuelve true UNA vez por turno (consume el cupo del reembolso).</summary>
    bool TryConsumeRefund();
}
