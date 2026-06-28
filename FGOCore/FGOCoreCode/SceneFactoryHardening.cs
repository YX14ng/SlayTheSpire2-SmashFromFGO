using System;
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
/// (<c>CreateCustomVisuals</c>), aquí NO hay hook de override, así que parcheamos los call-sites: un
/// transpiler cambia SOLO la llamada <c>Instantiate&lt;T&gt;</c> por
/// <c>NodeFactory&lt;T&gt;.CreateFromScene</c> (que usa el <c>_instance</c> de NUESTRA BaseLib, inmune
/// al clobber). Resultado idéntico en setups normales (mismo <c>CreateFromNode</c>). Con guarda: si
/// no encuentra el patrón exacto, no toca nada (no-op). Ver docs/REPORT-figure_Saya-baselib-conflict.md.
/// </summary>
internal static class SceneFactoryHardening
{
    /// Reemplaza `(PackedScene).Instantiate&lt;nodeType&gt;(GenEditState.Disabled)` por
    /// `NodeFactory&lt;nodeType&gt;.CreateFromScene((PackedScene))` en el IL del método.
    internal static IEnumerable<CodeInstruction> Swap(IEnumerable<CodeInstruction> instructions, Type nodeType)
    {
        var createFromScene = typeof(NodeFactory<>).MakeGenericType(nodeType)
            .GetMethod("CreateFromScene", new[] { typeof(PackedScene) });

        var codes = new List<CodeInstruction>(instructions);
        var patched = 0;
        if (createFromScene != null)
        {
            for (int i = 1; i < codes.Count; i++)
            {
                var c = codes[i];
                if ((c.opcode == OpCodes.Callvirt || c.opcode == OpCodes.Call)
                    && c.operand is MethodInfo mi
                    && mi.Name == "Instantiate"
                    && mi.DeclaringType == typeof(PackedScene)
                    && mi.IsGenericMethod
                    && mi.GetGenericArguments().Length == 1
                    && mi.GetGenericArguments()[0] == nodeType
                    && IsLdcI4Zero(codes[i - 1]))
                {
                    // quitar el push de GenEditState.Disabled (== 0, ahora innecesario para la
                    // factory estática) y reemplazar la llamada por CreateFromScene(PackedScene).
                    // Modificación in-place: preserva labels/branches de cada instrucción.
                    codes[i - 1].opcode = OpCodes.Nop;
                    codes[i - 1].operand = null;
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = createFromScene;
                    patched++;
                }
            }
        }
        if (patched == 0)
            MainFile.Logger.Warn($"SceneFactoryHardening: no se encontró Instantiate<{nodeType.Name}> para parchear (no-op).");
        return codes;
    }

    private static bool IsLdcI4Zero(CodeInstruction c)
        => c.opcode == OpCodes.Ldc_I4_0
        || (c.opcode == OpCodes.Ldc_I4 && c.operand is int v && v == 0)
        || (c.opcode == OpCodes.Ldc_I4_S && c.operand is sbyte s && s == 0);
}

[HarmonyPatch(typeof(NRestSiteCharacter), nameof(NRestSiteCharacter.Create))]
internal static class RestSiteCharacterFactoryPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        => SceneFactoryHardening.Swap(instructions, typeof(NRestSiteCharacter));
}

[HarmonyPatch(typeof(NMerchantRoom), "AfterRoomIsLoaded")]
internal static class MerchantRoomFactoryPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        => SceneFactoryHardening.Swap(instructions, typeof(NMerchantCharacter));
}
