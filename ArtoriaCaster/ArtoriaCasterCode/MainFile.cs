using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Combat;
using ArtoriaCaster.ArtoriaCasterCode.Cards.Special;
using ArtoriaCaster.ArtoriaCasterCode.Powers;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "ArtoriaCaster"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // FGOCore preloads every registered form's frames in background threads.
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/artoria_frames_caster.tres",
            $"{ResPath}/character/artoria_frames_berserker.tres",
            $"{ResPath}/character/artoria_frames_avalon.tres");

        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): al cruzar 100 NP,
        // AROUND CALIBURN: Desatado aparece GRATIS en la mano (Retain + Exhaust), igual que el «Mumyou
        // Desatado» de Okita. La carta lleva la ventana de crítico-en-cualquier-forma + el soporte
        // (Anti-Purga + estrellas), antes inline en este handler. Un marcador (NpManifestedPower) evita
        // duplicarla en el mismo pico; gastar por debajo de 100 lo re-arma. (Sin +1⚡/robar.)
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmManifest;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.ArtoriaCaster) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        await PowerCmd.Apply<NpManifestedPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<AroundCaliburnUnleashed>(creature);
    }

    private static async Task DisarmManifest(Creature creature)
    {
        if (creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Remove<NpManifestedPower>(creature);
        }
    }
}
