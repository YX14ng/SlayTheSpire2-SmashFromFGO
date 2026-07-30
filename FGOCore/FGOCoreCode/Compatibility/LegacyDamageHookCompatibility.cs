using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Compatibility;

public interface ILegacyDamageHooks
{
    decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource);
    decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource);
    decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource);
}

/// <summary>
/// Stable damage-hook contract used by FGO models on both game branches. MAIN dispatches here from
/// the five-argument overrides, while BETA passes its native <see cref="CardPlay"/> context either
/// through the six-argument overrides (beta build) or this compatibility patch (universal build).
/// </summary>
public interface IFgoDamageHooks
{
    decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay);

    decimal ModifyDamageMultiplicativeFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay);

    decimal ModifyDamageCapFgo(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay);
}

[HarmonyPatch]
internal static class LegacyDamageHookCompatibility
{
    private const BindingFlags PublicInstance = BindingFlags.Instance | BindingFlags.Public;

    private static MethodBase? FindBetaHook(string name, params Type[] parameterTypes) =>
        typeof(AbstractModel).GetMethod(name, PublicInstance, null, parameterTypes, null);

    private static bool NeedsUniversalBridge(MethodBase? betaHook)
    {
#if STS2_BETA
        // A beta-targeted build overrides the six-argument methods directly.
        return false;
#else
        return betaHook is not null;
#endif
    }

    [HarmonyPatch]
    private static class Additive
    {
        private static readonly MethodBase? BetaHook = FindBetaHook(nameof(AbstractModel.ModifyDamageAdditive),
            typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel),
            typeof(CardPlay));

        private static bool Prepare() => NeedsUniversalBridge(BetaHook);
        private static MethodBase? TargetMethod() => BetaHook;

        private static void Postfix(AbstractModel __instance, Creature? target, decimal amount, ValueProp props,
            Creature? dealer, CardModel? cardSource, CardPlay? cardPlay, ref decimal __result)
        {
            if (__instance is IFgoDamageHooks fgo)
                __result = fgo.ModifyDamageAdditiveFgo(target, amount, props, dealer, cardSource, cardPlay);
            else if (__instance is ILegacyDamageHooks legacy)
                __result = legacy.ModifyDamageAdditive(target, amount, props, dealer, cardSource);
        }
    }

    [HarmonyPatch]
    private static class Multiplicative
    {
        private static readonly MethodBase? BetaHook = FindBetaHook(nameof(AbstractModel.ModifyDamageMultiplicative),
            typeof(Creature), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel),
            typeof(CardPlay));

        private static bool Prepare() => NeedsUniversalBridge(BetaHook);
        private static MethodBase? TargetMethod() => BetaHook;

        private static void Postfix(AbstractModel __instance, Creature? target, decimal amount, ValueProp props,
            Creature? dealer, CardModel? cardSource, CardPlay? cardPlay, ref decimal __result)
        {
            if (__instance is IFgoDamageHooks fgo)
                __result = fgo.ModifyDamageMultiplicativeFgo(target, amount, props, dealer, cardSource, cardPlay);
            else if (__instance is ILegacyDamageHooks legacy)
                __result = legacy.ModifyDamageMultiplicative(target, amount, props, dealer, cardSource);
        }
    }

    [HarmonyPatch]
    private static class Cap
    {
        private static readonly MethodBase? BetaHook = FindBetaHook(nameof(AbstractModel.ModifyDamageCap),
            typeof(Creature), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay));

        private static bool Prepare() => NeedsUniversalBridge(BetaHook);
        private static MethodBase? TargetMethod() => BetaHook;

        private static void Postfix(AbstractModel __instance, Creature? target, ValueProp props,
            Creature? dealer, CardModel? cardSource, CardPlay? cardPlay, ref decimal __result)
        {
            if (__instance is IFgoDamageHooks fgo)
                __result = fgo.ModifyDamageCapFgo(target, props, dealer, cardSource, cardPlay);
            else if (__instance is ILegacyDamageHooks legacy)
                __result = legacy.ModifyDamageCap(target, props, dealer, cardSource);
        }
    }
}
