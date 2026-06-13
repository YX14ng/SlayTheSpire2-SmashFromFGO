using FGOCore.FGOCoreCode.Stars;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

        // Modelo de NP nuevo: cruzar 100 NP NO genera una carta-ulti gratis (eclipsaba a las
        // NP drafteadas). Abre la "Sentencia de la Reina": un TURNO DE DESCARGA de estrellas
        // — un Crítico gratis + un lote de Estrellas para volcar en Ataques Buster + recursos
        // (evoca el +300% star gather de su skill real "From the World's End"). Re-centrada en
        // Buster/crítico (rediseño 2026-06-13), no en detonar maldición.
        NpCharge.GaugeFilled += TryOpenNpWindow;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryOpenNpWindow(Creature creature)
    {
        if (creature.Player?.Character is not Character.MorganBerserker) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        // Marker: la Sentencia ya se disparó este pico (se re-arma al bajar < 100).
        await PowerCmd.Apply<NpManifestedPower>(creature, 1m, creature, null);

        // Un Crítico gratis (próximo Ataque Buster ×2) + un lote de Estrellas para más críticos.
        await PowerCmd.Apply<CritReadyPower>(creature, 1m, creature, null);
        await CritStars.Gain(creature, 40, null);

        // Devuelve recursos: arranca el turno grande, no lo reemplaza (modelo Phrolova).
        if (creature.Player != null)
        {
            await PlayerCmd.GainEnergy(1, creature.Player);
            await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, creature.Player);
        }
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Remove<NpManifestedPower>(creature);
        }
    }
}
