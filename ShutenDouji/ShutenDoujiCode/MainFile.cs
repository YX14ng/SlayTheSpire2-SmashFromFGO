using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using ShutenDouji.ShutenDoujiCode.Cards.Special;
using ShutenDouji.ShutenDoujiCode.Character;
using ShutenDouji.ShutenDoujiCode.Powers;

namespace ShutenDouji.ShutenDoujiCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "ShutenDouji";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        new Harmony(ModId).PatchAll();
        NpCharge.GaugeFilledWithContext += EnsureNpsInCombat;
        NpCharge.GaugeDroppedWithContext += DisarmManifest;
        FgoAttributes.RegisterOverride(ModelDb.GetId<Shuten>(), FgoAttribute.Earth);
    }

    public static async Task EnsureNpsInCombat(PlayerChoiceContext context, Creature creature)
    {
        if (creature.Player?.Character is not Shuten || creature.CombatState == null) return;
        if (!NpCharge.IsOvercharged(creature)) return;

        if (!creature.HasPower<DualNpManifestedPower>())
        {
            await PowerCmd.Apply<DualNpManifestedPower>(
                context, creature, 1m, creature, null, silent: true);
        }

        if (!HasLiveNp<SenjiBankoShinpenKidoku>(creature))
            await ManifestCards.ManifestToHand<SenjiBankoShinpenKidoku>(creature);
        if (!HasLiveNp<GohoShojoKuzuryuOsatsu>(creature))
            await ManifestCards.ManifestToHand<GohoShojoKuzuryuOsatsu>(creature);
    }

    public static Task EnsureNpsInCombat(Creature creature) =>
        EnsureNpsInCombat(new BlockingPlayerChoiceContext(), creature);

    public static async Task ExhaustSiblingNp(
        PlayerChoiceContext context, Creature creature, CardModel played)
    {
        var player = creature.Player;
        if (player == null) return;

        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var sibling in pile.GetPile(player).Cards
                         .Where(card => card != played && card is IShutenNpCard).ToList())
            {
                await CardCmd.Exhaust(context, sibling, skipVisuals: true);
            }
        }
    }

    private static bool HasLiveNp<T>(Creature creature) where T : CardModel
    {
        var player = creature.Player;
        if (player == null) return false;
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            if (pile.GetPile(player).Cards.Any(card => card is T)) return true;
        }
        return false;
    }

    private static async Task DisarmManifest(PlayerChoiceContext _, Creature creature)
    {
        if (creature.Player?.Character is not Shuten) return;
        if (creature.GetPower<DualNpManifestedPower>() is { } marker)
            await PowerCmd.Remove(marker);
    }
}
