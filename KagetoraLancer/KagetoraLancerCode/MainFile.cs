using Godot;
using HarmonyLib;
using KagetoraLancer.KagetoraLancerCode.Cards.Special;
using KagetoraLancer.KagetoraLancerCode.Character;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace KagetoraLancer.KagetoraLancerCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "KagetoraLancer";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        new Harmony(ModId).PatchAll();
        NpCharge.GaugeFilledWithContext += TryManifestNp;
        NpCharge.GaugeDroppedWithContext += DisarmManifest;

        FormVisuals.RegisterFramesWithSpriteTransform(
            ($"{ResPath}/character/kagetora_frames.tres", -70.7f, -316.1f, 1f),
            ($"{ResPath}/character/kenshin_frames.tres", 37.4f, -303.6f, 1f));

        // Kagetora/Kenshin usan Earth canónico, independientemente del tipo de sala.
        FgoAttributes.RegisterOverride(ModelDb.GetId<Kagetora>(), FgoAttribute.Earth);
    }

    private static Task TryManifestNp(PlayerChoiceContext _, Creature creature) =>
        EnsureNpInCombat(creature);

    public static async Task EnsureNpInCombat(Creature creature)
    {
        if (creature.Player?.Character is not Kagetora || creature.CombatState == null) return;
        if (!NpCharge.IsOvercharged(creature) || HasLiveNp(creature)) return;

        if (!creature.HasPower<NpManifestedPower>())
        {
            await PowerCmd.Apply<NpManifestedPower>(
                new BlockingPlayerChoiceContext(), creature, 1m, creature, null, silent: true);
        }

        if (creature.HasPower<KenshinFormPower>())
            await ManifestCards.ManifestToHand<BitenHassouShiranui>(creature);
        else
            await ManifestCards.ManifestToHand<BitenHassouKurumaGakariNoJin>(creature);
    }

    private static bool HasLiveNp(Creature creature)
    {
        var player = creature.Player;
        if (player == null) return false;
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var card in pile.GetPile(player).Cards)
            {
                if (card is IKagetoraNpCard) return true;
            }
        }
        return false;
    }

    private static async Task DisarmManifest(PlayerChoiceContext _, Creature creature)
    {
        if (creature.Player?.Character is not Kagetora) return;
        if (creature.GetPower<NpManifestedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}
