using FGOCore.FGOCoreCode.Combat;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using TiamatBeast.TiamatCode.Cards.Special;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    // Renombrado de "Tiamat" a "TiamatBeast" para coexistir con el otro Tiamat (D&D, mismo id "Tiamat")
    // en un mismo install. BaseLib carga la loc desde res://{id}/localization/, así que el namespace
    // res:// completo se renombró a res://TiamatBeast/ (assets + loc) — namespaces separados, cero colisión.
    public const string ModId = "TiamatBeast";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // Precarga de frames de las 2 formas (humano / Bestia). Los .tres se generan en el
        // pase de arte (ver DESIGN-TIAMAT.md / WORKFLOW-FGO §3).
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/tiamat_frames_human.tres",
            $"{ResPath}/character/tiamat_frames_beast.tres");

        // Modelo dos-pozas (rediseño, ver docs/REDESIGN-TIAMAT.md): a 100 NO se abre nada solo —
        // se MANIFIESTA en la mano la carta-NP de apertura «Nammu Dur-an-ki». El jugador decide
        // CUÁNDO jugarla: a 100 abre una ventana Bestia corta (1 turno), o banquea hasta 300 para
        // una larga (3). Toda la lógica de apertura (limpiar debuffs, AoE fijo + Sello, cambio a
        // Bestia, manifestar el mazo Bestia, abrir la ventana, devolver recursos) vive AHORA en la
        // carta, no acá. El marcador GenesisSpentPower evita re-manifestarla mientras sigas ≥100;
        // bajar < 100 (al consumirla con ConsumeAll) la re-arma para el próximo ciclo.
        NpCharge.GaugeFilled += TryManifestGenesis;
        NpCharge.GaugeDropped += RearmGenesis;
    }

    private static async Task TryManifestGenesis(Creature creature)
    {
        if (creature.Player?.Character is not Character.Tiamat) return;
        if (creature.HasPower<GenesisSpentPower>()) return;
        if (creature.CombatState == null || creature.Player == null) return;

        await PowerCmd.Apply<GenesisSpentPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);
        await ManifestCards.ManifestToHand<NammuDuranki>(creature);
    }

    private static async Task RearmGenesis(Creature creature)
    {
        if (creature.HasPower<GenesisSpentPower>())
        {
            await PowerCmd.Remove<GenesisSpentPower>(creature);
        }
    }
}
