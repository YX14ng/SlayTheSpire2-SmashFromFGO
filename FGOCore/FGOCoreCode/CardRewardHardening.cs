using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace FGOCore.FGOCoreCode;

/// <summary>
/// Toda recompensa de cartas del juego (fin de combate, eventos como "El futuro de las pociones",
/// rerolls de Driftwood, eventos de otros mods) pasa por el privado
/// <c>CardFactory.CreateForReward(player, blacklist, options)</c>, que elige las N opciones (típico 3)
/// UNA POR UNA metiendo cada elegida en <c>blacklist</c> para que sean distintas. Si el pool filtrado
/// menos la blacklist queda vacío, <c>Rng.NextItem</c> devuelve null y el método tira
/// <c>InvalidOperationException("couldn't generate a valid card")</c> → la pantalla de recompensa queda
/// rota ("无法选牌", reportado en Tiamat con el evento de pociones y con un evento de otro mod).
/// El juego vanilla nunca lo pisa porque cada personaje base tiene ≥3 cartas por (rareza × tipo); un
/// pool custom chico (Tiamat: 1 Poder infrecuente, 2 raras por tipo) lo pisa con cualquier filtro
/// rareza+tipo como el del evento de pociones.
///
/// Fix general: finalizer que, ante esa excepción, reintenta el MISMO método con blacklist vacía — la
/// recompensa puede ofrecer duplicados (igual que <c>GetForCombat</c>, que los admite por diseño) en vez
/// de reventar. Reutiliza el 100% de la lógica original (odds de rareza, hooks, filtro multiplayer);
/// el guard por hilo <c>_retrying</c> corta la recursión (el delegado re-entra por el detour de
/// Harmony). Si hasta
/// el reintento falla (pool genuinamente vacío), se propaga la excepción original como en vanilla.
/// Determinista en MP: la condición de fallo y el reintento consumen el mismo stream de RNG en todos
/// los clientes.
/// </summary>
[HarmonyPatch(typeof(CardFactory), "CreateForReward",
    new[] { typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardCreationOptions) })]
internal static class CardRewardHardening
{
    private delegate CardModel CreateForRewardDelegate(
        Player player, IEnumerable<CardModel> blacklist, CardCreationOptions options);

    private static readonly CreateForRewardDelegate Original = AccessTools.Method(
            typeof(CardFactory), "CreateForReward",
            [typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardCreationOptions)])
        .CreateDelegate<CreateForRewardDelegate>();

    [ThreadStatic]
    private static bool _retrying;

    private static Exception? Finalizer(Exception? __exception, ref CardModel? __result,
        Player player, CardCreationOptions options)
    {
        if (__exception is not InvalidOperationException || _retrying) return __exception;

        _retrying = true;
        try
        {
            __result = Original(player, Array.Empty<CardModel>(), options);
            return __result != null ? null : __exception;
        }
        catch
        {
            return __exception;
        }
        finally
        {
            _retrying = false;
        }
    }
}
