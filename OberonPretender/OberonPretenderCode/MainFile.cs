using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

        // (audit 2026-07-04) NO se registran frames de formas: los .tres por-forma todavia no
        // existen (pase de arte pendiente, modelos 2800100/2800110/2800120 — WORKFLOW-FGO 3).
        // Los FormPower devuelven FramesPath=null (mantienen el sprite base del visuals.tscn).

// Ulti AUTO-MANIFESTADA por forma (patron ArtoriaCaster, audit 2026-07-04): la Desatada de la
        // forma activa aparece en mano MIENTRAS tengas >=100 NP y NO haya ya una copia en tus pilas.
        // El patron viejo (marker + GaugeDropped) acumulaba copias Retain duplicadas: el cobro de
        // Deuda de fin de turno gasta NP -> GaugeDropped re-armaba el marker cada turno, y al volver
        // a 100 se manifestaba una SEGUNDA copia con la primera todavia en mano.
        NpCharge.GaugeFilled += TryManifestUlt;
    }

    private static Task TryManifestUlt(Creature creature) => EnsureUltInHand(creature);

    /// <summary>Manifiesta la Desatada de la forma ACTIVA si Oberon esta en combate con >=100 de
    /// Carga NP y no hay ya una copia en mano/robo/descarte. Idempotente: seguro de llamar en cada
    /// cruce de 100 y a inicio de turno (OberonFormPower.AfterSideTurnStart).</summary>
    public static async Task EnsureUltInHand(Creature creature)
    {
        if (creature.Player?.Character is not Character.Oberon) return;
        if (creature.CombatState == null || creature.Player == null) return;
        if (!NpCharge.IsOvercharged(creature)) return;

        var vortigern = creature.HasPower<VortigernPower>();
        if (HasUltInPiles(creature, vortigern)) return;

        CardModel card = vortigern
            ? creature.CombatState.CreateCard<LieLikeVortigernUnleashed>(creature.Player)
            : creature.CombatState.CreateCard<RyeRhymeGoodfellowUnleashed>(creature.Player);

        await ManifestCards.ManifestToHand(creature, card);
    }

    private static bool HasUltInPiles(Creature creature, bool vortigern)
    {
        var player = creature.Player;
        if (player == null) return false;
        // Mano + robo + descarte: con la mano llena el manifest se desvia al descarte (mismo caso
        // dedup de Artoria) — mirar solo la mano generaria copias extra.
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var c in pile.GetPile(player).Cards)
            {
                if (vortigern ? c is LieLikeVortigernUnleashed : c is RyeRhymeGoodfellowUnleashed) return true;
            }
        }
        return false;
    }
}
