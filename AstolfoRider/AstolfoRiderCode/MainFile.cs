using FGOCore.FGOCoreCode.Ritsu;
using STS2RitsuLib;
using AstolfoRider.AstolfoRiderCode.Cards.Special;
using AstolfoRider.AstolfoRiderCode.Character;
using AstolfoRider.AstolfoRiderCode.Powers;
using Godot;
using HarmonyLib;
using FGOCore.FGOCoreCode.Forms;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace AstolfoRider.AstolfoRiderCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "AstolfoRider";
    public const string ResPath = $"res://{ModId}";
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        RitsuLibFramework.CreateLogger(ModId);

    public static void Initialize()
    {
        FgoRitsuIntegration.RegisterCharacterMod<
            Astolfo,
            Relics.ReasonEvaporatedRelic,
            Relics.CompletelyEvaporatedReason,
            Cards.Basic.PaladinsHunch,
            Cards.Rare.PerfectImprovisation>(ModId, "reason_evaporated_relic");
        new Harmony(ModId).PatchAll();
        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/astolfo_frames.tres", -102f, -229f, 1f));
        NpCharge.GaugeFilledWithContext += EnsureNpInCombat;
        NpCharge.GaugeDroppedWithContext += DisarmManifest;
        FgoAttributes.RegisterOverride(ModelDb.GetId<Astolfo>(), FgoAttribute.Earth);
    }

    public static async Task EnsureNpInCombat(PlayerChoiceContext context, Creature creature)
    {
        if (creature.Player?.Character is not Astolfo || creature.CombatState == null) return;
        if (!NpCharge.IsOvercharged(creature)) return;
        if (!creature.HasPower<HippogriffManifestedPower>())
            await PowerCmd.Apply<HippogriffManifestedPower>(
                context, creature, 1m, creature, null, silent: true);
        if (!HasLiveNp(creature)) await ManifestCards.ManifestToHand<Hippogriff>(creature);
    }

    public static Task EnsureNpInCombat(Creature creature) =>
        EnsureNpInCombat(new BlockingPlayerChoiceContext(), creature);

    private static bool HasLiveNp(Creature creature)
    {
        var player = creature.Player;
        if (player == null) return false;
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
            if (pile.GetPile(player).Cards.Any(card => card is Hippogriff)) return true;
        return false;
    }

    private static async Task DisarmManifest(PlayerChoiceContext _, Creature creature)
    {
        if (creature.Player?.Character is not Astolfo) return;
        if (creature.GetPower<HippogriffManifestedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}
