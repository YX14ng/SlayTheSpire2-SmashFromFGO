using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace FGOCore.FGOCoreCode;

/// <summary>
/// DustyTome (la reliquia que ofrece el Ancient "Darv" en el Acto 2) elige una carta de rareza
/// <see cref="CardRarity.Ancient"/> al azar del pool del personaje:
/// <c>player.PlayerRng.Rewards.NextItem(ancientCards).Id</c>. <see cref="Rng.NextItem"/> devuelve
/// <c>null</c> sobre una colección VACÍA, así que <c>.Id</c> tira <c>NullReferenceException</c> para
/// cualquier personaje SIN cartas Ancient — o sea todos los Servants FGO menos Siegfried. Darv ofrece
/// DustyTome ~50% de las veces y llama <c>SetupForPlayer</c> mientras GENERA sus opciones, así que el
/// evento entero se cuelga ("solo el diálogo, no sale el reliquia, no se puede avanzar" — reportado en
/// FGOCore por jugadores en el Acto 2 / 达弗).
///
/// Fix: si el personaje no tiene cartas Ancient, caemos a sus Raras (en el espíritu de DustyTome = "ganás
/// una carta poderosa"), y si tampoco hay, a cualquier carta del pool. Así DustyTome siempre entrega una
/// carta válida de SU PROPIO pool en vez de crashear. Usa PlayerRng.Rewards (local, MP-safe) igual que el
/// original. Si el personaje SÍ tiene cartas Ancient (Siegfried), deja correr el original sin tocar nada.
///
/// Compatibilidad: Acheron instala otro prefix que repite el acceso inseguro a la lista vacía. Este
/// prefix debe ejecutarse primero; al devolver false para los personajes afectados, Harmony omite los
/// prefixes mutadores posteriores y evita que Acheron reabra el mismo crash.
/// </summary>
[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
internal static class DustyTomeHardening
{
    [HarmonyPriority(Priority.First)]
    [HarmonyBefore("Acheron")]
    private static bool Prefix(DustyTome __instance, Player player)
    {
        var pool = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .ToList();

        // El personaje tiene cartas Ancient: el original funciona, no lo tocamos.
        if (pool.Any(IsAncient)) return true;

        // Sin Ancient: Raras -> cualquier carta del pool. NextItem sobre esto NO es vacío.
        var fallback = pool.Where(c => c.Rarity == CardRarity.Rare).ToList();
        if (fallback.Count == 0) fallback = pool;
        if (fallback.Count == 0) return true; // pool vacío (no debería pasar): que corra el original

        __instance.AncientCard = player.PlayerRng.Rewards.NextItem(fallback)!.Id;
        return false; // saltea el original (evita el NextItem(vacío).Id)
    }

    private static bool IsAncient(CardModel c)
        => c.Rarity == CardRarity.Ancient && !ArchaicTooth.TranscendenceCards.Contains(c);
}
