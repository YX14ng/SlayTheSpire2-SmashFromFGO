using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// Adds the NP dupe option to a card reward screen, respecting the engine's hard cap of two
/// alternatives. Shared by every character's identity relic so the cap arithmetic lives once.
/// </summary>
public static class NpDupeAlternative
{
    /// <summary>Tope duro de <see cref="CardRewardAlternative.Generate"/> en MAIN y BETA: con más
    /// de 2 alternativas tira <c>InvalidOperationException</c> y voltea toda la card reward.</summary>
    private const int MaxAlternatives = 2;

    /// <summary>Id de la alternativa de reroll de Driftwood, tal como la crea <c>Generate</c>.</summary>
    private const string RerollOptionId = "REROLL";

    /// <summary>
    /// Agrega la opción de dupe si entra bajo el tope. Devuelve lo que debe devolver
    /// <c>TryModifyCardRewardAlternatives</c>.
    /// </summary>
    public static bool TryAdd(
        Player? owner, Player player, List<CardRewardAlternative> alternatives,
        string optionId, Func<Task> onSelect)
    {
        if (owner == null || owner != player) return false;
        if (!NpLevels.CanLevelUp(owner)) return false;

        // Hay que reservar el lugar de las alternativas que agreguen las reliquias POSTERIORES a la
        // nuestra: RunState.IterateHookListeners recorre Player.Relics en orden de obtención y la de
        // identidad es la starter, así que corre primera y todavía no ve lo que agreguen las demás.
        // La única vanilla que agrega una (SACRIFICE, sin condición) es Pael's Wing, en MAIN y BETA.
        // Las derretidas no cuentan: siguen en Player.Relics pero IterateHookListeners las filtra.
        var pending = 0;
        foreach (var relic in owner.Relics)
        {
            if (relic is PaelsWing && !relic.IsMelted) pending++;
        }

        // Con Driftwood el reroll se DESPLAZA en vez de cederle el turno. Ceder no funciona:
        // CardReward.OnSelect captura la lista UNA sola vez antes del loop y resuelve el índice
        // contra esa copia, mientras que Reroll() → Populate() solo refresca la PANTALLA con una
        // lista nueva; el clic sobre el gacha regenerado caería en el REROLL viejo → rerolls
        // gratis infinitos y gacha inalcanzable. Solo lo sacamos si de verdad no entran los dos.
        var rerollIndex = alternatives.FindIndex(alternative => alternative.OptionId == RerollOptionId);
        var withoutReroll = rerollIndex >= 0 ? alternatives.Count - 1 : alternatives.Count;
        var fitsWithReroll = alternatives.Count + pending + 1 <= MaxAlternatives;
        var fitsWithoutReroll = withoutReroll + pending + 1 <= MaxAlternatives;
        if (!fitsWithReroll && !fitsWithoutReroll)
        {
            // Ni cediendo entramos (Skip + Driftwood + Pael's Wing): ahí el vanilla se rompe SOLO —
            // Generate tira antes de mostrar nada, con mods o sin ellos. Podamos igual el reroll
            // como guarda de crash: el jugador pierde el reroll de esa reward en vez de la reward.
            if (rerollIndex >= 0 && withoutReroll + pending <= MaxAlternatives)
                alternatives.RemoveAt(rerollIndex);
            return false;
        }

        if (!fitsWithReroll) alternatives.RemoveAt(rerollIndex);

        alternatives.Add(new CardRewardAlternative(
            optionId, onSelect, PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }
}
