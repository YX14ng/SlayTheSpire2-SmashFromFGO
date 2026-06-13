using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
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

        // Modelo de NP nuevo (2026-06-12): cruzar 100 NP NO genera una carta-ulti gratis
        // (eclipsaba a las cartas NP drafteadas — feedback del usuario). Abre la
        // "Sentencia de la Reina": detona la Maldición de TODOS los enemigos a la vez
        // (el clímax del motor: cosechás lo que sembraste) + devuelve recursos. Las cartas
        // NP drafteadas (Roadless Camelot, etc.) son el clímax que elegís jugar aparte.
        NpCharge.GaugeFilled += TryOpenNpWindow;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryOpenNpWindow(Creature creature)
    {
        if (creature.Player?.Character is not Character.MorganBerserker) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        // Marker: la Sentencia ya se disparó este pico (se re-arma al bajar < 100).
        await PowerCmd.Apply<NpManifestedPower>(creature, 1m, creature, null);

        // Sentencia de la Reina: detoná la Maldición de TODOS los enemigos (clímax AoE).
        var ctx = new BlockingPlayerChoiceContext();
        foreach (var enemy in creature.CombatState.GetOpponentsOf(creature).ToList())
        {
            if (enemy.IsDead) continue;
            var consumed = await Curses.Consume(enemy, CursePower.MaxPerEnemy);
            if (consumed > 0)
            {
                await CreatureCmd.Damage(ctx, enemy, consumed,
                    ValueProp.Unpowered | ValueProp.SkipHurtAnim, creature, null);
            }
        }

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
