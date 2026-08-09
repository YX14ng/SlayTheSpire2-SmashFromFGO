using FGOCore.FGOCoreCode.Ritsu;
using STS2RitsuLib;
using Godot;
using HarmonyLib;
using BaseLib.Config;
using FGOCore.FGOCoreCode.Visuals;
using MegaCrit.Sts2.Core.Modding;

namespace FGOCore.FGOCoreCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "FGOCore"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        RitsuLibFramework.CreateLogger(ModId);

    public static void Initialize()
    {
        // Los reportes de jugadores llegan sin decir SO/runtime; el build Linux nativo tiene un
        // bridge C# con fallas conocidas y sus excepciones salen con el mensaje enmascarado.
        Logger.Info(
            $"Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription} | " +
            $"{System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
        FgoRitsuIntegration.Initialize();
        ModConfigRegistry.Register(ModId, new FgoVisualConfig());
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}
