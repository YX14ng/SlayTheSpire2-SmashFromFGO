using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// RelicModel.Pool hace First() sobre ModelDb.AllRelicPools y tira InvalidOperationException para
/// cualquier reliquia que no esté en un pool — el caso de TODAS las reliquias de personajes custom
/// (RitsuLib ya lo advierte al cargar: "not contained in any relic pool"). El juego llama Pool
/// incondicionalmente desde DynamicDescription/HoverTip, así que hoverear una reliquia FGO en el
/// inventario rompía el tooltip y llenaba el log (visto en reportes de jugadores, MAIN v0.107.1).
/// No registramos pools nuevos porque los RelicPool gobiernan los drops de recompensas; sólo
/// amortiguamos la lectura: reliquias que sí tienen pool no cambian en nada.
/// </summary>
[HarmonyPatch(typeof(RelicModel), "get_Pool")]
internal static class RelicPoolFallback
{
    private static readonly HashSet<string> LoggedRelicIds = [];

    [HarmonyFinalizer]
    private static Exception? SupplyFallbackPool(
        RelicModel __instance,
        Exception? __exception,
        ref RelicPoolModel __result)
    {
        // Sólo el First() sin match (reliquia sin pool). Cualquier otra excepción del juego o de
        // otro mod se propaga intacta (precedente DECISIONS: el guard de BaseLib 3.4.3 suprime
        // únicamente su excepción exacta).
        if (__exception is not InvalidOperationException) return __exception;

        // RelicModel.Owner hace AssertMutable() y TIRA CanonicalModelException en el modelo
        // canónico (hover en la biblioteca, fuera de una run) — mismo guard que usa el juego
        // en su call site de EnergyIconHelper: consultar Owner sólo si IsMutable.
        var fallback = (__instance.IsMutable ? __instance.Owner : null)?.Character?.RelicPool
            ?? ModelDb.AllRelicPools.FirstOrDefault();
        if (fallback is null) return __exception;

        __result = fallback;
        var id = __instance.Id.ToString();
        lock (LoggedRelicIds)
        {
            if (LoggedRelicIds.Add(id))
                MainFile.Logger.Info(
                    $"Relic '{id}' no pertenece a ningún relic pool; usando '{fallback.Id}' para su tooltip.");
        }

        return null;
    }
}
