using MegaCrit.Sts2.Core.Entities.Players;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// FGO dupe mechanic: the character's NP level (1-5) boosts their Noble Phantasm
/// cards' power. Dupes are rolled by giving up card rewards (see the character's
/// <see cref="INpLevelStore"/> relic). Tuned to not be tedious: 50% base chance,
/// +25% per consecutive failed roll (pity), reset on success.
/// <see cref="ILimitBreaker"/> relics (Holy Grail) raise the level cap.
/// </summary>
public static class NpLevels
{
    public const int BaseMaxLevel = 5;

    /// <summary>+15% NP card power per level above 1.</summary>
    public const decimal BonusPerLevel = 0.15m;

    private const int BaseChancePercent = 50;
    private const int PityChancePercent = 25;

    public static INpLevelStore? Store(Player player)
    {
        foreach (var relic in player.Relics)
        {
            if (relic is INpLevelStore store) return store;
        }
        return null;
    }

    public static int Get(Player? player)
    {
        if (player == null) return 1;
        return Math.Max(1, Store(player)?.NpLevel ?? 1);
    }

    public static int MaxLevel(Player player)
    {
        var max = BaseMaxLevel;
        foreach (var relic in player.Relics)
        {
            if (relic is ILimitBreaker breaker) max += breaker.ExtraNpLevels;
        }
        return max;
    }

    public static bool CanLevelUp(Player player)
    {
        var store = Store(player);
        return store != null && store.NpLevel < MaxLevel(player);
    }

    public static decimal Factor(Player? player) => 1m + BonusPerLevel * (Get(player) - 1);

    /// <summary>Scale an NP card's main amount by the owner's NP level.</summary>
    public static decimal Scale(Player? player, decimal amount) => Math.Round(amount * Factor(player));

    /// <summary>Roll the gacha. True = dupe obtained (level up, pity reset).</summary>
    public static bool TryRollDupe(Player player)
    {
        var store = Store(player);
        if (store == null || !CanLevelUp(player)) return false;

        var chance = BaseChancePercent + PityChancePercent * store.DupePity;
        if (player.RunState.Rng.CombatCardGeneration.NextInt(100) < chance)
        {
            store.NpLevel++;
            store.DupePity = 0;
            return true;
        }
        store.DupePity++;
        return false;
    }
}
