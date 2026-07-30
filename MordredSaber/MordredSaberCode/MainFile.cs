using Godot;
using HarmonyLib;
using FGOCore.FGOCoreCode.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MordredSaber.MordredSaberCode.Cards.Special;
using MordredSaber.MordredSaberCode.Powers;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "MordredSaber";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // Mordred SÍ tiene formas (3 looks de las ascensiones del modelo único 100900, vía
        // attach/detach del casco) y SÍ tiene ulti auto-manifestada (Clarent Blood Arthur a 100 NP).
        // FGOCore precarga las formas hermanas en solitario; en co-op las carga bajo demanda.
        FormVisuals.RegisterFramesWithSpriteX(
            ($"{ResPath}/character/mordred_frames.tres", 78.0f));

        // A 100 NP se MANIFIESTA gratis (Retain, Exhaust) la carta-ulti — «Interludio» si estás en
        // clímax Relámpago Carmesí, si no «Desatado» (que, al jugarse Enmascarada, primero arranca el
        // yelmo). El marcador evita re-manifestarla mientras sigas sobre 100; bajar < 100 la re-arma.
        NpCharge.GaugeFilledWithContext += TryManifestUlt;
        NpCharge.GaugeDroppedWithContext += RearmUlt;
    }

    private static async Task TryManifestUlt(PlayerChoiceContext choiceContext, Creature creature)
    {
        if (creature.Player?.Character is not Character.Mordred) return;
        if (creature.HasPower<ClarentManifestedPower>()) return;
        if (creature.CombatState == null || creature.Player == null) return;

        await PowerCmd.Apply<ClarentManifestedPower>(choiceContext, creature, 1m, creature, null);

        if (creature.HasPower<CrimsonLightningFormPower>())
        {
            await ManifestCards.ManifestToHand<ClarentBloodArthurInterlude>(creature);
        }
        else
        {
            await ManifestCards.ManifestToHand<ClarentBloodArthurUnleashed>(creature);
        }
    }

    private static async Task RearmUlt(PlayerChoiceContext _, Creature creature)
    {
        if (creature.HasPower<ClarentManifestedPower>())
        {
            await PowerCmd.Remove<ClarentManifestedPower>(creature);
        }
    }
}
