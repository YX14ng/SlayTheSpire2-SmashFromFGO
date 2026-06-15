using FGOCore.FGOCoreCode.Combat;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using OkitaSaber.OkitaSaberCode.Cards.Special;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "OkitaSaber";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // FGOCore precarga en hilos los frames de cada forma registrada. Okita tiene UN solo swap:
        // 102710 (traje blanco, modelo inicial) → 102720 (haori asagi) del clímax «Flor del Bakumatsu».
        // (Si okita_frames_haori.tres aún no existe, FormVisuals loguea y mantiene el sprite actual.)
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/okita_frames.tres",
            $"{ResPath}/character/okita_frames_haori.tres");

        // Ulti auto-manifestada (DESIGN-OKITA §5.5): al cruzar 100 NP, el «Mumyou Sandanzuki: Desatado»
        // aparece GRATIS en la mano (Retain + Exhaust). Un marcador (MumyouManifestedPower) evita
        // duplicarla en el mismo pico; gastar por debajo de 100 lo re-arma para el próximo.
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUlt;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.Okita) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<MumyouManifestedPower>()) return;

        await PowerCmd.Apply<MumyouManifestedPower>(creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<MumyouUnleashed>(creature);
    }

    private static async Task DisarmUlt(Creature creature)
    {
        if (creature.HasPower<MumyouManifestedPower>())
        {
            await PowerCmd.Remove<MumyouManifestedPower>(creature);
        }
    }
}
