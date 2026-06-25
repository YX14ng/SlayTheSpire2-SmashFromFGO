using System.Linq;
using FGOCore.FGOCoreCode.Combat;
using FGOCore.FGOCoreCode.Curses;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.ValueProps;
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

        // Modelo de NP nuevo: cruzar 100 NP NO genera una carta-ulti gratis (eclipsaba a las
        // NP drafteadas). Abre la "Sentencia de la Reina" (rediseño 2026-06-15: swap
        // Estrellas→Maldición): al cruzar 100 detona de una a TODOS los enemigos malditos por su
        // Maldición SIN consumirla (la cosecha masiva de la Reina; el campo sigue sembrado para
        // tus Ataques de Sentencia) + recursos. Re-armada al bajar < 100. Las cartas NP drafteadas
        // (Roadless Camelot, etc.) son el clímax que elegís jugar dentro.
        NpCharge.GaugeFilled += TryOpenNpWindow;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryOpenNpWindow(Creature creature)
    {
        if (creature.Player?.Character is not Character.MorganBerserker) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        // Marker: la Sentencia ya se disparó este pico (se re-arma al bajar < 100).
        await PowerCmd.Apply<NpManifestedPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);

        // Marcador de la ventana de 1 turno (sabor + tooltip; expira al fin de tu turno).
        await PowerCmd.Apply<QueensSentenceWindowPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);

        // Detonación AoE: cada enemigo maldito recibe daño = su Maldición, SIN consumirla.
        // Daño Unblockable|Unpowered (no escala con Fuerza ni lo frena el Bloqueo), como el tick de Maldición.
        foreach (var enemy in creature.CombatState.GetOpponentsOf(creature).Where(e => !e.IsDead).ToList())
        {
            var curse = Curses.Of(enemy);
            if (curse <= 0) continue;
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemy, curse,
                ValueProp.Unblockable | ValueProp.Unpowered, creature, null);
        }

        // Devuelve recursos: arranca el turno grande, no lo reemplaza (modelo Phrolova).
        // Kernel común factorizado en FGOCore (mismo GainEnergy(1) + Draw(1) guardado por Player != null).
        await NpWindow.ReturnResources(creature, 1, 1);
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Remove<NpManifestedPower>(creature);
        }
    }
}
