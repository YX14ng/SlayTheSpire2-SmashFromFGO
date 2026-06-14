using Godot;
using HarmonyLib;
using GilgameshArcher.GilgameshArcherCode.Cards.Special;
using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace GilgameshArcher.GilgameshArcherCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "GilgameshArcher";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // ENUMA ELISH: Desatado es una ulti AUTO-MANIFESTADA (DESIGN-GILGAMESH §6): a diferencia de
        // Siegfried (cuyo Balmung es una carta manual del pool), cruzar 100 de Carga NP genera la
        // carta-ulti a la mano (0⚡, Retain, Exhaust). Por eso SÍ se enganchan GaugeFilled/Dropped.
        //
        // NO se registran frames de FormVisuals: Gilgamesh tiene un solo modelo de batalla (200200,
        // §3.5) — el swap cosmético opcional con NP≥100 NO usa FormPower y no entra en la espina.
        NpCharge.GaugeFilled += TryManifestEnuma;
        NpCharge.GaugeDropped += DisarmEnumaMarker;
    }

    private static async Task TryManifestEnuma(Creature creature)
    {
        if (creature.Player?.Character is not Character.Gilgamesh) return;
        if (creature.HasPower<EnumaManifestedPower>()) return;          // ya se manifestó este pico
        if (creature.CombatState == null || creature.Player == null) return;

        // Marcador: la ulti ya se manifestó (se re-arma al bajar < 100, abajo).
        await PowerCmd.Apply<EnumaManifestedPower>(creature, 1m, creature, null);

        // El viento de la creación parte cielo y tierra: la carta-ulti aparece en mano, lista (Retain).
        var card = creature.CombatState.CreateCard<EnumaElishUnleashed>(creature.Player);
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 1.2f);
    }

    private static async Task DisarmEnumaMarker(Creature creature)
    {
        if (creature.HasPower<EnumaManifestedPower>())
        {
            await PowerCmd.Remove<EnumaManifestedPower>(creature);
        }
    }
}
