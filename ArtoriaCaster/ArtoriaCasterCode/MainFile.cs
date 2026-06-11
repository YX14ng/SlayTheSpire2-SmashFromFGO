using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
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

        // Ult cycle: the ult manifested at 100 NP matches the active form
        // («¿vestida de qué cruzo los 100?» — the design's second big decision):
        // Berserker -> HOPE WILL CAMELOT (execution), Caster/Avalon -> AROUND
        // CALIBURN (the wall). Fixed until the gauge drops below 100.
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.ArtoriaCaster) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        await PowerCmd.Apply<NpManifestedPower>(creature, 1m, creature, null);

        CardModel card;
        if (creature.HasPower<SummerBerserkerFormPower>())
        {
            card = creature.CombatState.CreateCard<Cards.Special.HopeWillCamelotUnleashed>(creature.Player);
        }
        else
        {
            card = creature.CombatState.CreateCard<Cards.Special.AroundCaliburnUnleashed>(creature.Player);
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
