using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// The vanilla run-history animation has a closed type switch for its five characters and returns
/// an empty sound list for every custom character. FGO characters use the existing generic slash
/// and heavy-hit clips so their history replay does not become silent.
/// </summary>
internal static class RunHistorySfxCompatibility
{
    private static bool IsFgo(CharacterModel character)
    {
        var coreAssemblyName = typeof(RunHistorySfxCompatibility).Assembly.GetName().Name;
        var characterAssembly = character.GetType().Assembly;
        return characterAssembly == typeof(RunHistorySfxCompatibility).Assembly ||
               characterAssembly.GetReferencedAssemblies().Any(reference =>
                   string.Equals(reference.Name, coreAssemblyName, StringComparison.Ordinal));
    }

    private static void SupplyFallback(
        CharacterModel character,
        ref List<string> result,
        string fallback)
    {
        if (result.Count == 0 && IsFgo(character))
            result = [fallback];
    }

    [HarmonyPatch(typeof(NMapPointHistoryEntry), "GetSmallHitSfx")]
    private static class SmallHitPatch
    {
        [HarmonyPostfix]
        private static void Postfix(CharacterModel character, ref List<string> __result) =>
            SupplyFallback(character, ref __result, "slash_attack.mp3");
    }

    [HarmonyPatch(typeof(NMapPointHistoryEntry), "GetBigHitSfx")]
    private static class BigHitPatch
    {
        [HarmonyPostfix]
        private static void Postfix(CharacterModel character, ref List<string> __result) =>
            SupplyFallback(character, ref __result, "heavy_attack.mp3");
    }
}
