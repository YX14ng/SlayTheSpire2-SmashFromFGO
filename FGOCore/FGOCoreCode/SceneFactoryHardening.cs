using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Utils.NodeFactories;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace FGOCore.FGOCoreCode;

/// <summary>
/// Robustez anti-conflicto para hoguera y tienda. El juego crea el personaje del rest-site y de la
/// tienda con <c>GetScene(path).Instantiate&lt;NRestSiteCharacter/NMerchantCharacter&gt;()</c>, que
/// depende del patch GLOBAL de conversión de escena de BaseLib — el MISMO mecanismo que en combate
/// puede ser clobbeado por otra BaseLib forkeada (p. ej. figure_Saya) → <c>InvalidCastException</c>
/// / pantalla negra al entrar a la hoguera o la tienda. A diferencia del combate
/// (<c>CreateCustomVisuals</c>), aquí NO hay hook de override, así que un transpiler reemplaza SOLO la
/// llamada <c>PackedScene.Instantiate</c> por <c>NodeFactory&lt;T&gt;.CreateFromScene</c> (la factory de
/// NUESTRA BaseLib, inmune al clobber). Resultado idéntico en setups normales (mismo
/// <c>CreateFromNode</c>).
///
/// El match es DELIBERADAMENTE laxo: cualquier llamada a un método <c>Instantiate</c> declarado en
/// <c>PackedScene</c> (genérico o no — el <c>Instantiate&lt;T&gt;</c> de Godot puede compilar a una
/// llamada no-genérica + cast, por eso la versión anterior no matcheaba y quedaba no-op). El helper
/// toma <c>(PackedScene, GenEditState)</c> igual que <c>Instantiate</c>, así la pila queda idéntica y
/// no hace falta tocar el push del <c>GenEditState</c>. Loguea cuántas llamadas parcheó (0 = no-op,
/// señal de que el IL cambió). Ver docs/REPORT-figure_Saya-baselib-conflict.md.
/// </summary>
internal static class SceneFactoryHardening
{
    private static IEnumerable<CodeInstruction> Swap(IEnumerable<CodeInstruction> instructions, MethodInfo helper, string label)
    {
        var codes = new List<CodeInstruction>(instructions);
        var patched = 0;
        foreach (var c in codes)
        {
            if ((c.opcode == OpCodes.Callvirt || c.opcode == OpCodes.Call)
                && c.operand is MethodInfo mi
                && mi.Name == "Instantiate"
                && mi.DeclaringType == typeof(PackedScene))
            {
                c.opcode = OpCodes.Call;
                c.operand = helper;
                patched++;
            }
        }
        if (patched == 0) MainFile.Logger.Warn($"SceneFactoryHardening({label}): no se encontró PackedScene.Instantiate (no-op).");
        else MainFile.Logger.Info($"SceneFactoryHardening({label}): parcheadas {patched} llamada(s) Instantiate -> factory.");
        return codes;
    }

    // Helpers con la MISMA firma que PackedScene.Instantiate(GenEditState): reciben el PackedScene
    // (el 'this' de la llamada de instancia) + el GenEditState (ignorado) y devuelven el nodo construido
    // por NUESTRA factory. Si el original era no-genérico + castclass, el cast posterior es no-op.
    internal static NRestSiteCharacter BuildRestSite(PackedScene scene, PackedScene.GenEditState _)
        => NodeFactory<NRestSiteCharacter>.CreateFromScene(scene);

    internal static NMerchantCharacter BuildMerchant(PackedScene scene, PackedScene.GenEditState _)
        => NodeFactory<NMerchantCharacter>.CreateFromScene(scene);

    internal static IEnumerable<CodeInstruction> SwapRest(IEnumerable<CodeInstruction> instructions)
        => Swap(instructions, AccessTools.Method(typeof(SceneFactoryHardening), nameof(BuildRestSite)), "rest");

    internal static IEnumerable<CodeInstruction> SwapMerchant(IEnumerable<CodeInstruction> instructions)
        => Swap(instructions, AccessTools.Method(typeof(SceneFactoryHardening), nameof(BuildMerchant)), "merchant");
}

[HarmonyPatch(typeof(NRestSiteCharacter), nameof(NRestSiteCharacter.Create))]
internal static class RestSiteFactoryPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        => SceneFactoryHardening.SwapRest(instructions);
}

[HarmonyPatch(typeof(NMerchantRoom), "AfterRoomIsLoaded")]
internal static class MerchantRoomFactoryPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        => SceneFactoryHardening.SwapMerchant(instructions);
}
