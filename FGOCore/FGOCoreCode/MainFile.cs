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

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        ModConfigRegistry.Register(ModId, new FgoVisualConfig());
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}
