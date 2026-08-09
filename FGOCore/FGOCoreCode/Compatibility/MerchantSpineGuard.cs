using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// El _Ready de NMerchantCharacter construye un MegaSpineBinding sobre GetChild(0) asumiendo un
/// SpineSprite. Las escenas de tienda FGO son raster (Node2D + AnimatedSprite2D con autoplay), y
/// el camino de auto-conversión de BaseLib (TryAutoConvert) no marca CreatedFromFactory, por lo
/// que la guardia SkipInitialAnimIfNotSpine de BaseLib 3.4.3 no corre y el juego crashea al entrar
/// a la tienda (InvalidOperationException: "Expected BoundObject to be a SpineSprite" y abort).
/// Mismo chequeo que usa el propio juego en NRestSiteCharacter.GetChildSpineNodes; idéntico en
/// MAIN v0.107.1 y BETA v0.110.1.
/// </summary>
internal static class MerchantSpineGuard
{
    internal static bool HasSpineChild(NMerchantCharacter merchant)
        => merchant.GetChildCount() > 0 && merchant.GetChild(0).GetClass() == "SpineSprite";
}

[HarmonyPatch(typeof(NMerchantCharacter), "_Ready")]
internal static class MerchantReadySpineGuard
{
    // Priority.Low: si la guardia propia de BaseLib 3.4.3 ya interceptó (camino CreateFromScene),
    // la nuestra ni corre. Sólo rescata el camino de auto-conversión que hoy crashea.
    [HarmonyPriority(Priority.Low)]
    [HarmonyPrefix]
    private static bool SkipSpineBindingForRasterScenes(NMerchantCharacter __instance)
        => MerchantSpineGuard.HasSpineChild(__instance);
    // false = saltear el original; el AnimatedSprite2D arranca solo con autoplay="idle".
}

[HarmonyPatch(typeof(NMerchantCharacter), nameof(NMerchantCharacter.PlayAnimation))]
internal static class MerchantPlayAnimationSpineGuard
{
    // NGameOverScreen llama PlayAnimation("die") si el jugador muere en la tienda; sin esta
    // guardia ese camino re-crea el MegaSpineBinding y crashea igual que _Ready.
    [HarmonyPriority(Priority.Low)]
    [HarmonyPrefix]
    private static bool PlayRasterAnimation(NMerchantCharacter __instance, string anim)
    {
        if (MerchantSpineGuard.HasSpineChild(__instance)) return true;

        if (__instance.FindChild("Sprite", recursive: true, owned: false) is AnimatedSprite2D sprite
            && sprite.SpriteFrames is not null)
        {
            // Mapeo de BaseLib: el loop de reposo Spine equivale a nuestra animación "idle".
            var name = anim == "relaxed_loop" ? "idle" : anim;
            if (sprite.SpriteFrames.HasAnimation(name)) sprite.Play(name);
        }

        return false;
    }
}
