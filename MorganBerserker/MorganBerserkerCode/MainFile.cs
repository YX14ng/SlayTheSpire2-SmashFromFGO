using System.Linq;
using FGOCore.FGOCoreCode.Ritsu;
using STS2RitsuLib;
using FGOCore.FGOCoreCode.Combat;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MorganBerserker.MorganBerserkerCode.Cards.Special;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "MorganBerserker"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        RitsuLibFramework.CreateLogger(ModId);

    public static void Initialize()
    {
        FgoRitsuIntegration.RegisterCharacterMod<
            Character.MorganBerserker,
            Relics.QueensScepter,
            Relics.WorldsEndCoronation,
            Cards.Basic.LanceOfTheWorldsEnd,
            Cards.Rare.FromTheWorldsEnd>(ModId, "queens_scepter");
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // FGOCore preloads sibling forms in single-player; co-op loads them on demand to cap VRAM.
        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/morgan_frames_queen.tres", 59.2f, -216.4f, 0.981333f),
            ($"{ResPath}/character/morgan_frames_aesc.tres", 66.4f, -220.8f, 1.006667f),
            ($"{ResPath}/character/morgan_frames_winter.tres", 52.8f, -204.6f, 0.901333f));

        // Ulti auto-manifestada: al cruzar 100 NP, SENTENCIA DE LA REINA: Desatado aparece GRATIS
        // en la mano (Retain + Exhaust). Un marcador (NpManifestedPower) evita duplicarla en el
        // mismo pico; gastar por debajo de 100 lo re-arma. REDESIGN-MORGAN-V2 §3.4 (2026-08-15,
        // parche J3-1): Morgan es EXCEPCIÓN DOCUMENTADA a la convención «solo dan la carta» de
        // 2026-06-26 — su ventana devuelve +1⚡ y roba 1, y el cetro/Ancient re-arman la
        // Metamorfosis (M3). Una cosecha por pico: sin ×2 ni detonación AoE fuera de la carta.
        NpCharge.GaugeFilledWithContext += TryManifestUlt;
        NpCharge.GaugeDroppedWithContext += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(PlayerChoiceContext choiceContext, Creature creature)
    {
        if (creature.Player?.Character is not Character.MorganBerserker) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<NpManifestedPower>()) return;

        await PowerCmd.Apply<NpManifestedPower>(choiceContext, creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<QueensSentenceUnleashed>(creature);

        // Ventana §3.4: +1⚡ y robá 1. El robo copia el guard anti-soft-lock del cetro: GaugeFilled
        // puede disparar a MITAD de la resolución de una carta, y un reshuffle tomaría del descarte
        // la carta en curso ("must be added to a CombatState") — robar SOLO si el mazo tiene cartas.
        if (creature.Player is { } player)
        {
            await PlayerCmd.GainEnergy(1, player);
            var inDeck = player.PlayerCombatState?.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            if (inDeck > 0)
            {
                await CardPileCmd.Draw(choiceContext, 1, player);
            }

            // M3: el cetro (o el Ancient que lo reemplaza) re-arma la Metamorfosis gratis al llegar
            // a 100 — 1 vez por pico (este handler ya está gateado por NpManifestedPower).
            if (player.Relics.Any(r => r is Relics.QueensScepter or Relics.WorldsEndCoronation))
            {
                await ManifestCards.ManifestToHand<QueensMetamorphosis>(creature, 1.0f);
            }
        }
    }

    private static async Task DisarmUltMarker(PlayerChoiceContext _, Creature creature)
    {
        if (creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Remove<NpManifestedPower>(creature);
        }
    }
}
