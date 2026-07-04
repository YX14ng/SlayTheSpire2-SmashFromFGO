using System.Collections.Generic;
using System.Linq;
using FGOCore.FGOCoreCode.Bond;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Marcador: carta incolora (meme/CE) que SOLO le sirve a un personaje FGO porque depende de la Carga NP
/// (Kaleidoscope/Prisma Cosmos/Formal Craft/Imaginary Element: dan o amplifican Carga NP). Están en el
/// <c>ColorlessCardPool</c> (pool compartido) para que cualquier Servant las pueda draftear, PERO un
/// personaje NO-FGO (vanilla u otro mod) no tiene medidor de NP → la carta es basura y molesta al ofrecerse
/// (reporte de player: "non FGO character can get colorless card related to NP charge is really annoying").
/// Los patches de abajo las excluyen de las ofertas (recompensas y tienda) cuando el jugador no es FGO.
/// </summary>
public interface INpDependentColorless;

internal static class ColorlessNpGating
{
    private static readonly Dictionary<System.Type, bool> FgoByCharacterType = [];

    /// <summary>Identidad ESTABLE (audit 2026-07-04): un personaje es FGO si su CharacterModel viene de
    /// un assembly que referencia FGOCore (los 10 mods lo hacen; vanilla y otros mods no). Antes se
    /// detectaba por tener una BondRelic — estado mutable: si un efecto remueve/reemplaza la reliquia
    /// starter (p.ej. Touch of Orobas), el personaje "dejaba de ser FGO" a mitad de la run. Cacheado por
    /// tipo (la reflexión corre una vez por personaje por sesión).</summary>
    internal static bool IsFgo(Player player)
    {
        var t = player.Character.GetType();
        if (!FgoByCharacterType.TryGetValue(t, out var fgo))
        {
            var self = typeof(ColorlessNpGating).Assembly;
            fgo = t.Assembly == self
                  || System.Linq.Enumerable.Any(t.Assembly.GetReferencedAssemblies(), a => a.Name == self.GetName().Name);
            FgoByCharacterType[t] = fgo;
        }
        return fgo;
    }
}

/// <summary>Tienda: el slot de carta incolora sale de <c>CreateForMerchant(player, colorlessPool, rarity)</c>.
/// Filtramos las cartas NP-dependientes del pool cuando el jugador no es FGO (queda el resto del incoloro
/// vanilla, así que nunca vacía el slot).</summary>
[HarmonyPatch(typeof(CardFactory), nameof(CardFactory.CreateForMerchant),
    new[] { typeof(Player), typeof(IEnumerable<CardModel>), typeof(CardRarity) })]
internal static class MerchantColorlessNpGate
{
    private static void Prefix(Player player, ref IEnumerable<CardModel> options)
    {
        if (ColorlessNpGating.IsFgo(player)) return;
        options = options.Where(c => c is not INpDependentColorless);
    }
}

/// <summary>Recompensas / cartas incoloras dadas por eventos o reliquias pasan por
/// <c>CardCreationOptions.GetPossibleCards(player)</c>. Sacamos las NP-dependientes del resultado si el
/// jugador no es FGO.</summary>
[HarmonyPatch(typeof(CardCreationOptions), nameof(CardCreationOptions.GetPossibleCards))]
internal static class RewardColorlessNpGate
{
    private static void Postfix(Player player, ref IEnumerable<CardModel> __result)
    {
        if (ColorlessNpGating.IsFgo(player)) return;
        __result = __result.Where(c => c is not INpDependentColorless);
    }
}
