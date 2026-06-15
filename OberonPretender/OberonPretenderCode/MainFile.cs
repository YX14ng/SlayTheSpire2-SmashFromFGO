using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Combat;
using OberonPretender.OberonPretenderCode.Cards.Special;
using OberonPretender.OberonPretenderCode.Powers;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "OberonPretender";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // Precarga en background de los frames de las 3 formas (Rey / Invierno / Vortigern).
        // Los .tres se generan en el pase de arte (modelos 2800100/2800110/2800120, WORKFLOW-FGO 3).
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/oberon_frames_king.tres",
            $"{ResPath}/character/oberon_frames_winter.tres",
            $"{ResPath}/character/oberon_frames_vortigern.tres");

        // Ulti AUTO-MANIFESTADA por forma (DESIGN-OBERON 5/6.5, modelo de carta-token en mano):
        // a 100 NP se genera la Desatada de la forma activa -- Rey/Invierno -> Rye Rhyme Goodfellow,
        // Vortigern -> Lie Like Vortigern. Queda en mano (Event 0, Retain+Exhaust) hasta jugarse;
        // se re-arma al gastar el medidor por debajo de 100.
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUlt;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.Oberon) return;
        if (creature.HasPower<UltManifestedPower>()) return;

        // Marca el pico (se re-arma al bajar < 100). Idempotente por pico.
        await PowerCmd.Apply<UltManifestedPower>(creature, 1m, creature, null);

        // La Desatada depende de la forma activa: Vortigern usa la suya; Rey/Invierno la del cuento.
        // El helper de FGOCore (ManifestCards) factoriza el CreateCard ya hecho + AddGeneratedCardToCombat
        // + PreviewCardPileAdd que estaba duplicado en los MainFile del ecosistema; la elección por forma
        // (la única parte mod-local) queda en el caller, que pasa la carta ya creada.
        CardModel card = creature.HasPower<VortigernPower>()
            ? creature.CombatState.CreateCard<LieLikeVortigernUnleashed>(creature.Player)
            : creature.CombatState.CreateCard<RyeRhymeGoodfellowUnleashed>(creature.Player);

        await ManifestCards.ManifestToHand(creature, card);
    }

    private static async Task DisarmUlt(Creature creature)
    {
        if (creature.HasPower<UltManifestedPower>())
        {
            await PowerCmd.Remove<UltManifestedPower>(creature);
        }
    }
}
