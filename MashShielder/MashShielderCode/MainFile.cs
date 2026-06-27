using FGOCore.FGOCoreCode.Combat;
using Godot;
using HarmonyLib;
using MashShielder.MashShielderCode.Cards.Special;
using MashShielder.MashShielderCode.Powers;
using MashShielder.MashShielderCode.Powers.Forms;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;

namespace MashShielder.MashShielderCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "MashShielder"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        // FGOCore preloads every registered form's frames in background threads.
        FormVisuals.RegisterFrames(
            $"{ResPath}/character/mash_frames_base.tres",
            $"{ResPath}/character/mash_frames_ortinax.tres",
            $"{ResPath}/character/mash_frames_paladin.tres");

        // Ulti auto-manifestada (consistencia con los otros Servants, 2026-06-26): al cruzar
        // 100 NP, la NP correspondiente a la FORMA activa aparece GRATIS en la mano (Retain +
        // Exhaust), igual que el «Mumyou Desatado» de Okita. Un marcador (CamelotManifestedPower,
        // genérico «ult manifestada» — nombre histórico, NO renombrar: rompe saves) evita
        // duplicarla en el mismo pico; gastar por debajo de 100 lo re-arma para el próximo.
        // (Sin +1⚡/robar: los 8 personajes solo dan la carta, sin recursos extra.)
        // Mapeo forma → NP (Mooncell/wiki FGO):
        //   Shielder (escudo base) → LORD CAMELOT (理想之城, la NP icónica del escudo)
        //   Ortinax  (armadura + cañón) → BLACK BARREL (黑桶)
        //   Paladin  (forma plena)  → LORD CHALDEAS (罗德·迦勒底亚斯)
        NpCharge.GaugeFilled += TryManifestUlt;
        NpCharge.GaugeDropped += DisarmUltMarker;
    }

    private static async Task TryManifestUlt(Creature creature)
    {
        if (creature.Player?.Character is not Character.MashShielder) return;
        if (creature.CombatState == null) return;
        if (creature.HasPower<CamelotManifestedPower>()) return;

        // Una sola ult por pico, sin importar la forma: el marcador genérico se aplica igual.
        await PowerCmd.Apply<CamelotManifestedPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);

        // Gatear la NP por la forma activa (los docstrings viejos tenían Shielder/Paladin invertidos).
        if (creature.HasPower<OrtinaxFormPower>())
        {
            await ManifestCards.ManifestToHand<BlackBarrelUnleashed>(creature);
        }
        else if (creature.HasPower<PaladinFormPower>())
        {
            await ManifestCards.ManifestToHand<LordChaldeasUnleashed>(creature);
        }
        else
        {
            await ManifestCards.ManifestToHand<LordCamelotUnleashed>(creature);
        }
    }

    private static async Task DisarmUltMarker(Creature creature)
    {
        if (creature.HasPower<CamelotManifestedPower>())
        {
            await PowerCmd.Remove<CamelotManifestedPower>(creature);
        }
    }
}
