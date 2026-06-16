using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace CoopDeterminismPatch;

/// <summary>
/// Mod parche: arregla la divergencia de estado (desconexion) de co-op causada por mods de terceros
/// que eligen relics/powers con RNG no sincronizada con la simulacion de combate. Carga DESPUES de
/// Ultraman (dependencies en el manifest) y aplica sus Harmony patches al inicializarse.
/// </summary>
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "CoopDeterminismPatch";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        Logger.Info("Co-op determinism patches applied.");
    }
}
