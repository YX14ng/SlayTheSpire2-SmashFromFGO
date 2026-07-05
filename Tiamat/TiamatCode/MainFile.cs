using FGOCore.FGOCoreCode.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using TiamatBeast.TiamatCode.Cards.Special;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    // Renombrado de "Tiamat" a "TiamatBeast" para coexistir con el otro Tiamat (D&D, mismo id "Tiamat")
    // en un mismo install. BaseLib carga la loc desde res://{id}/localization/, así que el namespace
    // res:// completo se renombró a res://TiamatBeast/ (assets + loc) — namespaces separados, cero colisión.
    public const string ModId = "TiamatBeast";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // Precarga de frames de las 2 formas (humano / Bestia). Los .tres se generan en el
        // pase de arte (ver DESIGN-TIAMAT.md / WORKFLOW-FGO §3).
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/tiamat_frames_human.tres",
            $"{ResPath}/character/tiamat_frames_beast.tres");

// Modelo dos-pozas (rediseno, ver docs/REDESIGN-TIAMAT.md): a 100 NO se abre nada solo —
        // se MANIFIESTA en mano la carta-NP de apertura «Nammu Dur-an-ki» MIENTRAS tengas >=100 y no
        // haya ya una copia en tus pilas ni una ventana Bestia activa. (Audit 2026-07-05: el patron
        // viejo marker+GaugeDropped BRICKEABA el medidor — si la ventana expiraba sin jugar Pluma, el
        // medidor quedaba >=100 sin volver a cruzar el umbral y Nammu no se re-manifestaba nunca mas.)
        // Se re-chequea en el cruce de 100 y a inicio de cada turno (TiamatFormPower).
        NpCharge.GaugeFilled += TryManifestGenesis;
    }

    private static Task TryManifestGenesis(Creature creature) => EnsureGenesisInHand(creature);

    /// <summary>Manifiesta Nammu Dur-an-ki si Tiamat esta en combate con >=100 de Carga NP, sin
    /// ventana Bestia activa y sin una copia ya en mano/robo/descarte. Idempotente.</summary>
    public static async Task EnsureGenesisInHand(Creature creature)
    {
        if (creature.Player?.Character is not Character.Tiamat) return;
        if (creature.CombatState == null || creature.Player == null) return;
        // Dentro de la ventana no se re-abre (re-meteria el mazo Bestia, re-cleansearia, etc.).
        if (creature.HasPower<TiamatBeastWindowPower>()) return;
        if (!NpCharge.IsOvercharged(creature)) return;
        if (HasGenesisInPiles(creature)) return;

        await ManifestCards.ManifestToHand<NammuDuranki>(creature);
    }

    private static bool HasGenesisInPiles(Creature creature)
    {
        var player = creature.Player;
        if (player == null) return false;
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var c in pile.GetPile(player).Cards)
            {
                if (c is NammuDuranki) return true;
            }
        }
        return false;
    }
}
