using FGOCore.FGOCoreCode.Memes;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
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

    // --- Consolation prizes (estilo el mod Miyabi): el dupe fallido NO se siente
    // tirado a la basura. La recompensa escala con la piedad ACUMULADA tras este
    // fallo (a más fallos seguidos, mejor el premio de consuelo). Tuneable. ---

    /// <summary>Oro de consuelo en piedad baja (0-1).</summary>
    private const int ConsolationGold = 30;

    /// <summary>Cantidad de Afilado (Sharp) al encantar en piedad media (2-3).</summary>
    private const int ConsolationSharpAmount = 1;

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

    /// <summary>
    /// Igual que <see cref="TryRollDupe"/> pero, en la rama de fallo, entrega un premio de
    /// consolación que escala con la piedad acumulada (estilo el mod Miyabi). El dupe seguido
    /// vale algo aunque no salga; la piedad ya sube la próxima probabilidad además del consuelo.
    /// Devuelve true = dupe obtenido (sin consolación: ganaste el premio gordo).
    /// </summary>
    public static async Task<bool> TryRollDupeWithConsolation(Player player)
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
        await GrantConsolation(player, store.DupePity);
        return false;
    }

    /// <summary>
    /// Premio de consuelo de un dupe fallido. Escala con <paramref name="pity"/> (la piedad YA
    /// incrementada por este fallo): bajo = oro; medio = mejora/encanta una carta del mazo;
    /// alto = elegís una carta colorless para sumar al mazo. Todo fuera de combate
    /// (<see cref="BlockingPlayerChoiceContext"/>); estamos dentro de la pantalla de recompensa
    /// de carta, así que NO se abre un set de recompensas anidado: la elección es inline.
    /// </summary>
    private static async Task GrantConsolation(Player player, int pity)
    {
        if (pity <= 1)
        {
            // Piedad baja (0-1): un puñado de oro. Discreto, siempre útil.
            await PlayerCmd.GainGold(ConsolationGold, player);
            return;
        }

        if (pity <= 3)
        {
            // Piedad media (2-3): mejorá una carta del mazo. Si no hay nada mejorable
            // (todo al tope), caé a encantar una con Afilado para que no quede sin premio.
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1);
            var selected = await CardSelectCmd.FromDeckForUpgrade(player, prefs);
            var card = selected.FirstOrDefault();
            if (card != null)
            {
                CardCmd.Upgrade(card);
                return;
            }

            // Fallback: encantar (Afilado) una carta de ataque del mazo. FromDeckForEnchantment
            // ya filtra a las cartas encantables; si no hay ninguna, el bucle no itera (patrón GnarledHammer).
            var sharp = ModelDb.Enchantment<Sharp>();
            var enchantPrefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
            foreach (var enchantCard in await CardSelectCmd.FromDeckForEnchantment(player, sharp, ConsolationSharpAmount, enchantPrefs))
            {
                CardCmd.Enchant(sharp.ToMutable(), enchantCard, ConsolationSharpAmount);
                CardCmd.Preview(enchantCard);
            }
            return;
        }

        // Piedad alta (4+): el consuelo más jugoso — elegí una carta colorless de cortesía
        // y se suma al mazo. Inline con FromChooseACardScreen (no un RewardsSet anidado).
        var options = new List<CardModel>
        {
            player.RunState.CreateCard<GoldenApple>(player),
            player.RunState.CreateCard<TenPullSummon>(player),
        };
        var chosen = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), options, player, false);
        if (chosen != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosen, PileType.Deck), 1.2f, CardPreviewStyle.EventLayout);
        }
    }
}
