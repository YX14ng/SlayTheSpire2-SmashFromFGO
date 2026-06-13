using Godot;
using HarmonyLib;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

        // Modelo de NP nuevo (2026-06-12): cruzar 100 NP NO genera una carta-ulti gratis
        // (eclipsaba a las cartas NP drafteadas — feedback del usuario). Abre la VENTANA
        // "Baluarte Absoluto" 1 turno (potencia el mazo + devuelve recursos); las cartas
        // NP drafteadas (Lord Camelot, etc.) son el clímax que elegís jugar DENTRO de ella.
        // Gastar por debajo de 100 re-arma la ventana para el próximo pico.
        NpCharge.GaugeFilled += TryOpenNpWindow;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryOpenNpWindow(Creature creature)
    {
        if (creature.Player?.Character is not Character.MashShielder) return;
        if (creature.HasPower<CamelotManifestedPower>()) return;

        // Marker: la ventana ya se abrió este pico (se re-arma al bajar < 100).
        await PowerCmd.Apply<CamelotManifestedPower>(creature, 1m, creature, null);
        await PowerCmd.Apply<AbsoluteBulwarkWindowPower>(creature, 1m, creature, null);

        // Devuelve recursos: arranca el turno grande, no lo reemplaza (modelo Phrolova).
        if (creature.Player != null)
        {
            await PlayerCmd.GainEnergy(1, creature.Player);
            await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, creature.Player);
        }
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<CamelotManifestedPower>())
        {
            await PowerCmd.Remove<CamelotManifestedPower>(creature);
        }
    }
}
