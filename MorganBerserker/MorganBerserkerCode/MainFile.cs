using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
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

        // Ult cycle: the ult manifested at 100 NP matches the active form (her real
        // FGO kit): Fairy Queen / Winter Queen -> ROADLESS CAMELOT, Rain Witch ->
        // MEMORY OF LONDINIUM. The generated card stays fixed if the form changes.
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.MorganBerserker) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        await PowerCmd.Apply<NpManifestedPower>(creature, 1m, creature, null);

        CardModel card;
        if (creature.HasPower<Powers.Forms.RainWitchFormPower>())
        {
            card = creature.CombatState.CreateCard<Cards.Special.MemoryOfLondiniumUnleashed>(creature.Player);
        }
        else
        {
            card = creature.CombatState.CreateCard<Cards.Special.RoadlessCamelotUnleashed>(creature.Player);
        }
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 1.5f);
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Remove<NpManifestedPower>(creature);
        }
    }
}
