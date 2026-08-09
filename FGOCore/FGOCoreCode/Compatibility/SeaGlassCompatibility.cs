using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Relics;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// Sea Glass concatena el ID del personaje a su clave de titulo, pero el juego solo incluye claves
/// para los personajes vanilla. Para los Servants usamos el titulo generico ya localizado del
/// propio relic y evitamos que Orobas termine en LocException al construir la opcion.
/// </summary>
[HarmonyPatch(typeof(SeaGlass), nameof(SeaGlass.Title), MethodType.Getter)]
internal static class SeaGlassCompatibility
{
    private static readonly string[] FgoCharacterPrefixes =
    [
        "MASHSHIELDER-",
        "MORGANBERSERKER-",
        "ARTORIACASTER-",
        "MORDREDSABER-",
        "GILGAMESHARCHER-",
        "OKITASABER-",
        "OBERONPRETENDER-",
        "SIEGFRIEDSABER-",
        "TIAMATBEAST-",
        "KAGETORALANCER-",
        "SHUTENDOUJI-",
        "ASTOLFORIDER-"
    ];

    [HarmonyPostfix]
    private static void UseGenericTitleForFgoCharacters(SeaGlass __instance, ref LocString __result)
    {
        var characterEntry = __instance.CharacterId?.Entry;
        if (characterEntry == null ||
            !FgoCharacterPrefixes.Any(prefix => characterEntry.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        __result = new LocString("relics", $"{__instance.Id.Entry}.title");
    }
}
