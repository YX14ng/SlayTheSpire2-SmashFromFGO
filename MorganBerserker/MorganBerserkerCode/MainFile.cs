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

        // FGOCore preloads sibling forms in single-player; co-op loads them on demand to cap VRAM.
        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/morgan_frames_queen.tres", 59.2f, -216.4f, 0.981333f),
            ($"{ResPath}/character/morgan_frames_aesc.tres", 66.4f, -220.8f, 1.006667f),
            ($"{ResPath}/character/morgan_frames_winter.tres", 52.8f, -204.6f, 0.901333f));

        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): al cruzar 100 NP,
        // SENTENCIA DE LA REINA: Desatado aparece GRATIS en la mano (Retain + Exhaust), igual que el
        // «Mumyou Desatado» de Okita. La carta lleva la detonación AoE de Maldición (antes inline en
        // este handler). Un marcador (NpManifestedPower) evita duplicarla en el mismo pico; gastar por
        // debajo de 100 lo re-arma. (Sin +1⚡/robar: los 8 personajes solo dan la carta.)
        NpCharge.GaugeFilledWithContext += TryManifestUlt;
        NpCharge.GaugeDroppedWithContext += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(PlayerChoiceContext choiceContext, Creature creature)
    {
        if (creature.Player?.Character is not Character.MorganBerserker) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        await PowerCmd.Apply<NpManifestedPower>(choiceContext, creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<QueensSentenceUnleashed>(creature);
    }

    private static async Task DisarmUltMarker(PlayerChoiceContext _, Creature creature)
    {
        if (creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Remove<NpManifestedPower>(creature);
        }
    }
}
