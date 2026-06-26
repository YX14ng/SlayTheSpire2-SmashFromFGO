using FGOCore.FGOCoreCode.Combat;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MorganBerserker.MorganBerserkerCode.Cards.Special;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "MorganBerserker"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // FGOCore preloads every registered form's frames in background threads.
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/morgan_frames_queen.tres",
            $"{ResPath}/character/morgan_frames_aesc.tres",
            $"{ResPath}/character/morgan_frames_winter.tres");

        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): al cruzar 100 NP,
        // SENTENCIA DE LA REINA: Desatado aparece GRATIS en la mano (Retain + Exhaust), igual que el
        // «Mumyou Desatado» de Okita. La carta lleva la detonación AoE de Maldición (antes inline en
        // este handler). Un marcador (NpManifestedPower) evita duplicarla en el mismo pico; gastar por
        // debajo de 100 lo re-arma. (Sin +1⚡/robar: los 8 personajes solo dan la carta.)
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.MorganBerserker) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        await PowerCmd.Apply<NpManifestedPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<QueensSentenceUnleashed>(creature);
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Remove<NpManifestedPower>(creature);
        }
    }
}
