using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace OberonPretender.OberonPretenderCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "OberonPretender";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // Oberon no tiene formas (modelo de batalla único 2800100) ni ulti auto-manifestada:
        // su NP (Balmung) es una carta manual vía ConsumeAllForNpCard. Por eso NO se enganchan
        // NpCharge.GaugeFilled/Dropped ni FormVisuals.RegisterFrames (cf. Tiamat). DESIGN-SIEGFRIED §2/§4.
    }
}
