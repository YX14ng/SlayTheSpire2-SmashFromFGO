using FGOCore.FGOCoreCode.Combat;
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

        // Siegfried no tiene formas (modelo de batalla único 100800), por eso NO se engancha
        // FormVisuals.RegisterFrames (cf. Tiamat).
        //
        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): al cruzar 100 NP,
        // «BALMUNG: Desatado» aparece GRATIS en la mano (Retain + Exhaust), igual que el «Mumyou
        // Desatado» de Okita / la «Sentencia de la Reina: Desatado» de Morgan. Un marcador
        // (SiegfriedNpManifestedPower) evita duplicarla en el mismo pico; gastar por debajo de 100
        // lo re-arma. La carta-NP rara «Balmung» (2⚡, comprada, con refund EX) sigue existiendo en
        // el pool aparte; ésta es la manifestación gratis del medidor.
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUlt;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.Siegfried) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<SiegfriedNpManifestedPower>()) return;

        await PowerCmd.Apply<SiegfriedNpManifestedPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<BalmungUnleashed>(creature);
    }

    private static async Task DisarmUlt(Creature creature)
    {
        if (creature.HasPower<SiegfriedNpManifestedPower>())
        {
            await PowerCmd.Remove<SiegfriedNpManifestedPower>(creature);
        }
    }
}
