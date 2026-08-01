using FGOCore.FGOCoreCode.Combat;
using FGOCore.FGOCoreCode.Forms;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using SiegfriedSaber.SiegfriedSaberCode.Cards.Special;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "SiegfriedSaber";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // Siegfried no tiene formas de gameplay, pero registra su set visual único para que
        // FGOCore pueda elegir entre los recursos de 768 y 1024 px al crear el combate.
        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/siegfried_frames.tres", -27.4f, -294.5f, 1.004f));
        //
        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): al cruzar 100 NP,
        // «BALMUNG: Desatado» aparece GRATIS en la mano (Retain + Exhaust), igual que el «Mumyou
        // Desatado» de Okita / la «Sentencia de la Reina: Desatado» de Morgan. Un marcador
        // (SiegfriedNpManifestedPower) evita duplicarla en el mismo pico; gastar por debajo de 100
        // lo re-arma. La carta-NP rara «Balmung» (2⚡, comprada, con refund EX) sigue existiendo en
        // el pool aparte; ésta es la manifestación gratis del medidor.
        NpCharge.GaugeFilledWithContext += TryManifestUlt;
        NpCharge.GaugeDroppedWithContext += DisarmUlt;
    }

    private static async Task TryManifestUlt(PlayerChoiceContext choiceContext, Creature creature)
    {
        if (creature.Player?.Character is not Character.Siegfried) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<SiegfriedNpManifestedPower>()) return;

        await PowerCmd.Apply<SiegfriedNpManifestedPower>(choiceContext, creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<BalmungUnleashed>(creature);
    }

    private static async Task DisarmUlt(PlayerChoiceContext _, Creature creature)
    {
        if (creature.HasPower<SiegfriedNpManifestedPower>())
        {
            await PowerCmd.Remove<SiegfriedNpManifestedPower>(creature);
        }
    }
}
