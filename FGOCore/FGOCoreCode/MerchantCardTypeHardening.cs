using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode;

/// <summary>
/// La tienda arma un slot de carta por cada tipo en <c>_coloredCardTypes</c> (Ataque/Habilidad/Poder) y
/// para cada uno llama <c>CardFactory.CreateForMerchant(player, pool, type)</c>. Ese método rueda una
/// rareza y busca la primera rareza permitida con AL MENOS una carta de ese tipo; si el personaje NO
/// tiene ninguna carta no-básica de ese tipo, <c>GetNextAllowedRarity</c> devuelve <c>None</c> y
/// <c>CreateForMerchant</c> tira <c>InvalidOperationException("Can't generate valid rarity for merchant
/// card type ...")</c> → la sala de tienda revienta → PANTALLA NEGRA al entrar a la tienda (reportado y
/// confirmado por log). Tiamat tiene 0 cartas de tipo Poder (hueco de contenido; los otros 9 tienen ≥6),
/// así que su tienda crashea SIEMPRE en el slot de Poder.
///
/// Fix general (cubre Tiamat y cualquier personaje futuro con un tipo faltante): si el pool no tiene
/// NINGUNA carta no-básica del <paramref name="type"/> pedido, lo remapeamos al tipo del que el personaje
/// tiene MÁS cartas, así el slot muestra una carta real en vez de tirar. Los personajes que sí tienen ese
/// tipo pasan sin cambios. NO es la conversión de escena (SceneFactoryHardening) — esa era otra cosa y ya
/// aplica bien; esta es la causa real del "进商店黑屏".
/// </summary>
[HarmonyPatch(typeof(CardFactory), nameof(CardFactory.CreateForMerchant),
    new[] { typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardType) })]
internal static class MerchantCardTypeHardening
{
    private static void Prefix(IEnumerable<CardModel> options, ref CardType type)
    {
        var requested = type;                            // no se puede usar el ref `type` dentro de un lambda
        var nonBasic = options.Where(c => c.Rarity != CardRarity.Basic).ToList();
        if (nonBasic.Count == 0) return;                 // sin no-básicas: dejá que el original haga lo suyo
        if (nonBasic.Any(c => c.Type == requested)) return; // el personaje SÍ tiene cartas de este tipo: ok

        // El personaje no tiene ninguna carta no-básica de `type` (p.ej. Tiamat sin Poder) -> remapeá al
        // tipo del que tiene MÁS, para que el slot se llene con una carta real en vez de crashear.
        var fallback = nonBasic
            .GroupBy(c => c.Type)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .First();
        type = fallback;
    }
}
