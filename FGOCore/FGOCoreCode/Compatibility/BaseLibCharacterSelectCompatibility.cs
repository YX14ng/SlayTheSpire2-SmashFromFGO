using System;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// BaseLib 3.4.3 fue compilado contra BETA, donde StartRunLobby.LocalPlayer devuelve
/// StartRunLobbyPlayer. MAIN 0.107.1 expone la misma propiedad con retorno LobbyPlayer; como el
/// retorno forma parte de la firma CLR, su postfix de OnEmbarkPressed lanza MissingMethodException.
/// El embarque original ya terminó en ese punto, por lo que neutralizamos únicamente esa excepción
/// binaria conocida y dejamos que cualquier otro error se propague normalmente.
/// </summary>
[HarmonyPatch(typeof(NCharacterSelectScreen), "OnEmbarkPressed")]
internal static class BaseLibCharacterSelectCompatibility
{
    private const string BrokenPatchType =
        "BaseLib.Patches.UI.CharacterSelectStartingRelicsPatch";
    private const string MissingMember = "StartRunLobby.get_LocalPlayer()";

    private static int _warningLogged;

    [HarmonyFinalizer]
    [HarmonyPriority(Priority.Last)]
    private static Exception? SuppressBrokenBaseLibPostfix(Exception? __exception)
    {
        if (!IsKnownBaseLibMismatch(__exception)) return __exception;

        if (Interlocked.Exchange(ref _warningLogged, 1) == 0)
        {
            MainFile.Logger.Warn(
                "BaseLib 3.4.3 intento usar la firma BETA de StartRunLobby.LocalPlayer en MAIN; " +
                "FGOCore neutralizo el fallo de su UI de reliquias iniciales para permitir el embarque.");
        }

        return null;
    }

    private static bool IsKnownBaseLibMismatch(Exception? exception)
    {
        if (exception is not MissingMethodException ||
            !exception.Message.Contains(MissingMember, StringComparison.Ordinal))
        {
            return false;
        }

        MethodBase? target = exception.TargetSite;
        if (string.Equals(target?.DeclaringType?.FullName, BrokenPatchType, StringComparison.Ordinal) &&
            string.Equals(target?.Name, "OnEmbarkPressedPostfix", StringComparison.Ordinal))
        {
            return true;
        }

        return exception.StackTrace?.Contains(
            $"{BrokenPatchType}.OnEmbarkPressedPostfix", StringComparison.Ordinal) == true;
    }
}
