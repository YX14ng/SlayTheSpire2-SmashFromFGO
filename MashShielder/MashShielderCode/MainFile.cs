using FGOCore.FGOCoreCode.Combat;
using Godot;
using HarmonyLib;
using MashShielder.MashShielderCode.Cards.Special;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;

namespace MashShielder.MashShielderCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "MashShielder"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // FGOCore preloads every registered form's frames in background threads.
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/mash_frames_base.tres",
            $"{ResPath}/character/mash_frames_ortinax.tres",
            $"{ResPath}/character/mash_frames_paladin.tres");

        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): al cruzar
        // 100 NP, LORD CAMELOT: Desatado aparece GRATIS en la mano (Retain + Exhaust), igual
        // que el «Mumyou Desatado» de Okita. Un marcador (CamelotManifestedPower) evita
        // duplicarla en el mismo pico; gastar por debajo de 100 lo re-arma para el próximo.
        // (Sin +1⚡/robar: los 8 personajes solo dan la carta, sin recursos extra.)
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.MashShielder) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<CamelotManifestedPower>()) return;

        await PowerCmd.Apply<CamelotManifestedPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<LordCamelotUnleashed>(creature);
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<CamelotManifestedPower>())
        {
            await PowerCmd.Remove<CamelotManifestedPower>(creature);
        }
    }
}
