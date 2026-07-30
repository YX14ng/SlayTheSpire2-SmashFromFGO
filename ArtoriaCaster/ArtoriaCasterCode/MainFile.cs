using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Combat;
using ArtoriaCaster.ArtoriaCasterCode.Cards.Special;
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

        // FGOCore preloads sibling forms in single-player; co-op loads them on demand to cap VRAM.
        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/artoria_frames_caster.tres", 48.2f, -213.1f, 0.961333f),
            ($"{ResPath}/character/artoria_frames_berserker.tres", 28.4f, -192.0f, 0.861333f),
            ($"{ResPath}/character/artoria_frames_avalon.tres", 84.4f, -247.1f, 1.289333f));

        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): AROUND CALIBURN:
        // Desatado aparece GRATIS en la mano (Retain + Exhaust) MIENTRAS tengas >=100 de Carga NP y NO la
        // tengas ya en mano. La carta lleva la ventana de crítico-en-cualquier-forma + el soporte
        // (Anti-Purga + estrellas). El dedup es "la carta ya está en mano" (antes: marcador + obligarte a
        // gastar por debajo de 100 para re-armar), así un overshoot del Kaleidoscope que te deja sentado
        // en >=100, o perder la carta por exhaust, NO te dejan atascado sin ulti (reporte del jugador).
        // Se re-chequea en el cruce de 100 (GaugeFilled, mid-turno) y a inicio de cada turno
        // (ArtoriaFormPower.AfterSideTurnStart, cubre la carga inicial por Kaleidoscope que no cruza).
        NpCharge.GaugeFilledWithContext += TryManifestUlt;
    }

    private static Task TryManifestUlt(PlayerChoiceContext _, Creature creature) => EnsureUltInHand(creature);

    /// <summary>Manifiesta AROUND CALIBURN: Desatado en la mano si la criatura es Castoria, está en
    /// combate, tiene >=100 de Carga NP y NO tiene ya la ulti en mano. Idempotente: seguro de llamar
    /// en cada cruce de 100 y a inicio de turno.</summary>
    public static async Task EnsureUltInHand(Creature creature)
    {
        if (creature.Player?.Character is not Character.ArtoriaCaster) return;
        if (creature.CombatState == null) return;
        if (!NpCharge.IsOvercharged(creature)) return;       // < 100 de Carga NP
        if (HasUltInHand(creature)) return;                  // ya la tenés en mano

        await ManifestCards.ManifestToHand<AroundCaliburnUnleashed>(creature);
    }

    private static bool HasUltInHand(Creature creature)
    {
        var player = creature.Player;
        if (player == null) return false;
        // Mano + robo + descarte (audit 2026-07-04): con la MANO LLENA el manifest se desvía al
        // descarte (CardPileCmd redirige el add) — dedupear solo contra la mano generaba una copia
        // extra de la ulti en cada re-chequeo (cruce de 100 / inicio de turno).
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var c in pile.GetPile(player).Cards)
            {
                if (c is AroundCaliburnUnleashed) return true;
            }
        }
        return false;
    }
}
