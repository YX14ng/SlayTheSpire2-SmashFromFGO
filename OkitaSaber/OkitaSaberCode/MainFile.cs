using FGOCore.FGOCoreCode.Ritsu;
using STS2RitsuLib;
using FGOCore.FGOCoreCode.Combat;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using OkitaSaber.OkitaSaberCode.Cards.Special;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "OkitaSaber";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        RitsuLibFramework.CreateLogger(ModId);

    public static void Initialize()
    {
        FgoRitsuIntegration.RegisterCharacterMod<
            Character.Okita,
            Relics.HaoriAsagi,
            Relics.FlowerOfImperialCapital,
            Cards.Basic.Shukuchi,
            Cards.Rare.InfiniteInstant>(ModId, "haori_asagi");
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // La forma final conserva el set animado oficial de Okita; su identidad mecanica se
        // representa con el poder permanente y su icono, sin duplicar los frames de combate.
        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/okita_frames.tres", 2.3f, -357.2f, 0.956f));

        // Ulti auto-manifestada (DESIGN-OKITA §5.5): al cruzar 100 NP, el «Mumyou Sandanzuki: Desatado»
        // aparece GRATIS en la mano (Retain + Exhaust). Un marcador (MumyouManifestedPower) evita
        // duplicarla en el mismo pico; gastar por debajo de 100 lo re-arma para el próximo.
        NpCharge.GaugeFilledWithContext += TryManifestUlt;
        NpCharge.GaugeDroppedWithContext += DisarmUlt;
    }

    private static async Task TryManifestUlt(PlayerChoiceContext choiceContext, Creature creature)
    {
        if (creature.Player?.Character is not Character.Okita) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<MumyouManifestedPower>()) return;

        await PowerCmd.Apply<MumyouManifestedPower>(choiceContext, creature, 1m, creature, null);

        await ManifestCards.ManifestToHand<MumyouUnleashed>(creature);
    }

    private static async Task DisarmUlt(PlayerChoiceContext _, Creature creature)
    {
        if (creature.HasPower<MumyouManifestedPower>())
        {
            await PowerCmd.Remove<MumyouManifestedPower>(creature);
        }
    }
}
