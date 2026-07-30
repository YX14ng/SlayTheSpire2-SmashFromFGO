using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// Makes repeated upgrades useful for FGO Skills and Powers without turning every reusable card
/// into a deterministic zero-cost loop. The first upgrade remains entirely card-authored.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.UpgradeInternal))]
internal static class InfiniteUpgradeCompatibility
{
    private sealed record UpgradeState(int EnergyCost, Dictionary<string, decimal> DynamicValues);

    private static readonly HashSet<string> CharacterAssemblies = new(StringComparer.Ordinal)
    {
        "MashShielder",
        "MorganBerserker",
        "ArtoriaCaster",
        "MordredSaber",
        "GilgameshArcher",
        "OkitaSaber",
        "OberonPretender",
        "SiegfriedSaber",
        "TiamatBeast",
        "KagetoraLancer",
        "ShutenDouji",
        "AstolfoRider"
    };

    private static void Prefix(CardModel __instance, out UpgradeState? __state)
    {
        __state = null;
        if (__instance.CurrentUpgradeLevel < 1 || !IsCharacterCard(__instance)) return;

        __state = new UpgradeState(
            __instance.EnergyCost.GetWithModifiers(CostModifiers.None),
            __instance.DynamicVars.ToDictionary(pair => pair.Key, pair => pair.Value.BaseValue,
                StringComparer.Ordinal));
    }

    private static void Postfix(CardModel __instance, UpgradeState? __state)
    {
        if (__state is null) return;

        ClampRepeatedReductions(__instance, __state.DynamicValues);

        var cost = __instance.EnergyCost;
        if (cost.CostsX || cost.Canonical <= 0 || !IsRewardPoolRarity(__instance.Rarity)) return;

        // An authored cost upgrade is valid once, but repeating it indefinitely would also make
        // Attacks and reusable draw/energy Skills free. Restore this upgrade's starting cost and
        // then apply the shared repeated-upgrade rule below.
        var currentBaseCost = cost.GetWithModifiers(CostModifiers.None);
        var authoredCostReductionWasSuppressed = currentBaseCost < __state.EnergyCost;
        if (authoredCostReductionWasSuppressed)
        {
            cost.SetCustomBaseCost(__state.EnergyCost);
            currentBaseCost = __state.EnergyCost;
        }

        var minimumCost = __instance.Type switch
        {
            CardType.Power => 0,
            CardType.Skill when __instance.Keywords.Contains(CardKeyword.Exhaust) => 0,
            CardType.Skill => 1,
            _ => currentBaseCost
        };

        if (currentBaseCost > minimumCost)
            cost.UpgradeBy(-1);
        else if (authoredCostReductionWasSuppressed)
            cost.FinalizeUpgrade();
    }

    private static void ClampRepeatedReductions(
        CardModel card, IReadOnlyDictionary<string, decimal> previousValues)
    {
        foreach (var (name, dynamicVar) in card.DynamicVars)
        {
            var minimum = MinimumFor(name);
            if (minimum is null || dynamicVar.BaseValue >= minimum.Value) continue;

            var wasAlreadyAtMinimum = previousValues.TryGetValue(name, out var previous) &&
                                      previous == minimum.Value;
            dynamicVar.UpgradeValueBy(minimum.Value - dynamicVar.BaseValue);
            if (wasAlreadyAtMinimum) dynamicVar.FinalizeUpgrade();
        }
    }

    private static decimal? MinimumFor(string dynamicVarName)
    {
        if (dynamicVarName.EndsWith("Cost", StringComparison.Ordinal) ||
            dynamicVarName is "Debt" or "HpLoss")
            return 0m;

        if (dynamicVarName is "Divisor" or "Turn") return 1m;
        return null;
    }

    private static bool IsCharacterCard(CardModel card) =>
        CharacterAssemblies.Contains(card.GetType().Assembly.GetName().Name ?? string.Empty);

    private static bool IsRewardPoolRarity(CardRarity rarity) => rarity is
        CardRarity.Basic or CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare;
}
