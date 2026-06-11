using Godot;
using HarmonyLib;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

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

        // Ult cycle: reaching 100 NP manifests Lord Camelot for free; spending below
        // 100 re-arms it so the next full gauge manifests it again.
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.MashShielder) return;
        if (creature.HasPower<CamelotManifestedPower>()) return;

        await PowerCmd.Apply<CamelotManifestedPower>(creature, 1m, creature, null);

        // The ult matches the active form, like her FGO kit: Shielder → LORD CHALDEAS
        // (her true NP), Ortinax → BLACK BARREL (her Lostbelt armament), Paladin →
        // LORD CAMELOT (the shield's true name). The generated card stays fixed if
        // the form changes afterwards — the decision is in which form you cross 100.
        CardModel card;
        if (creature.HasPower<Powers.Forms.OrtinaxFormPower>())
        {
            card = creature.CombatState.CreateCard<Cards.Special.BlackBarrelUnleashed>(creature.Player);
        }
        else if (creature.HasPower<Powers.Forms.ShielderFormPower>())
        {
            card = creature.CombatState.CreateCard<Cards.Special.LordChaldeasUnleashed>(creature.Player);
        }
        else
        {
            card = creature.CombatState.CreateCard<Cards.Special.LordCamelotUnleashed>(creature.Player);
        }
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 1.5f);
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<CamelotManifestedPower>())
        {
            await PowerCmd.Remove<CamelotManifestedPower>(creature);
        }
    }
}
