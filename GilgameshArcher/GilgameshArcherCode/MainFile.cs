using FGOCore.FGOCoreCode.Ritsu;
using STS2RitsuLib;
using Godot;
using HarmonyLib;
using FGOCore.FGOCoreCode.Forms;
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

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        RitsuLibFramework.CreateLogger(ModId);

    public static void Initialize()
    {
        FgoRitsuIntegration.RegisterCharacterMod<
            Character.Gilgamesh,
            Relics.BabIlu,
            Relics.EaSwordOfRupture,
            Cards.Basic.GateOfBabylon,
            Cards.Rare.KingsArsenal>(ModId, "bab_ilu");
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // El render oficial de Gilgamesh mide 867 px: Alta conserva su resolución nativa y usa una
        // compensación específica en vez del factor estándar 768/1024.
        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/gilgamesh_frames.tres", 4.2f, -175.9f, 0.504621f, 768f / 867f));

        // ENUMA ELISH: Desatado es una ulti AUTO-MANIFESTADA (DESIGN-GILGAMESH §6): a diferencia de
        // Siegfried (cuyo Balmung es una carta manual del pool), cruzar 100 de Carga NP genera la
        // carta-ulti a la mano (0⚡, Retain, Exhaust). Por eso SÍ se enganchan GaugeFilled/Dropped.
        //
        // Gilgamesh tiene un solo modelo de batalla (200200, §3.5); el registro anterior permite
        // que el selector visual lo actualice sin introducir una forma de gameplay artificial.
        NpCharge.GaugeFilledWithContext += TryManifestEnuma;
        NpCharge.GaugeDroppedWithContext += DisarmEnumaMarker;
    }

    private static async Task TryManifestEnuma(PlayerChoiceContext choiceContext, Creature creature)
    {
        if (creature.Player?.Character is not Character.Gilgamesh) return;
        if (creature.HasPower<EnumaManifestedPower>()) return;          // ya se manifestó este pico
        if (creature.CombatState == null || creature.Player == null) return;

        // Marcador: la ulti ya se manifestó (se re-arma al bajar < 100, abajo).
        await PowerCmd.Apply<EnumaManifestedPower>(choiceContext, creature, 1m, creature, null);

        // El viento de la creación parte cielo y tierra: la carta-ulti aparece en mano, lista (Retain).
        // Helper compartido de FGOCore (antes: CreateCard + AddGeneratedCardToCombat + PreviewCardPileAdd).
        await FGOCore.FGOCoreCode.Combat.ManifestCards.ManifestToHand<EnumaElishUnleashed>(creature, 1.2f);
    }

    private static async Task DisarmEnumaMarker(PlayerChoiceContext _, Creature creature)
    {
        if (creature.HasPower<EnumaManifestedPower>())
        {
            await PowerCmd.Remove<EnumaManifestedPower>(creature);
        }
    }
}
