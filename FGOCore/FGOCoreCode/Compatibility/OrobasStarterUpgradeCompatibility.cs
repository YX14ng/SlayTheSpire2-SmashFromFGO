using System.Reflection;
using FGOCore.FGOCoreCode.Np;
using FGOCore.FGOCoreCode.Ritsu;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// Repara partidas guardadas entre la preparacion y la obtencion de Orobas: si una version vieja
/// ya habia almacenado Circlet, usa el mapeo oficial que cada personaje registro en RitsuLib.
/// </summary>
[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.AfterObtained))]
internal static class PreparedOrobasUpgradeCompatibility
{
    private static readonly FieldInfo UpgradedRelicField =
        AccessTools.Field(typeof(TouchOfOrobas), "_upgradedRelic");

    [HarmonyPrefix]
    private static void RefreshLegacyCircletChoice(TouchOfOrobas __instance)
    {
        var starter = __instance.StarterRelic is { } starterId
            ? ModelDb.GetById<RelicModel>(starterId)
            : __instance.Owner.Relics.FirstOrDefault(relic => relic.Rarity == RelicRarity.Starter);
        if (starter == null
            || __instance.UpgradedRelic != ModelDb.Relic<Circlet>().Id
            || !FgoRitsuIntegration.TryGetOrobasUpgrade(starter.GetType(), out var upgradedRelicType))
            return;

        var replacement = ModelDb.GetById<RelicModel>(ModelDb.GetId(upgradedRelicType));
        UpgradedRelicField.SetValue(__instance, replacement.Id);
        MainFile.Logger.Info(
            $"Touch of Orobas: reparado destino legado Circlet para {starter.Id} -> {replacement.Id}.");
    }
}

/// <summary>
/// Algunas starters FGO tambien son el almacen persistente de nivel NP. Como RelicCmd.Replace crea
/// otro objeto, copiamos ese estado al reemplazo Ancient para no reiniciar NP ni la piedad de dupes.
/// </summary>
[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Replace))]
internal static class FgoRelicReplacementStateCompatibility
{
    [HarmonyPrefix]
    private static void CaptureNpState(RelicModel original, RelicModel replace, out int[]? __state)
    {
        __state = original is INpLevelStore source && replace is INpLevelStore
            ? [source.NpLevel, source.DupePity]
            : null;
    }

    [HarmonyPostfix]
    private static void RestoreNpState(ref Task<RelicModel> __result, int[]? __state)
    {
        if (__state == null) return;
        __result = AwaitAndRestore(__result, __state[0], __state[1]);
    }

    private static async Task<RelicModel> AwaitAndRestore(
        Task<RelicModel> replacementTask,
        int npLevel,
        int dupePity)
    {
        var replacement = await replacementTask;
        if (replacement is INpLevelStore target)
        {
            target.NpLevel = npLevel;
            target.DupePity = dupePity;
        }
        return replacement;
    }
}
