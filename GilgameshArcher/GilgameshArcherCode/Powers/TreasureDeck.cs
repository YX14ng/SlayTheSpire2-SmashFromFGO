using FGOCore.FGOCoreCode.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using GilgameshArcher.GilgameshArcherCode.Cards.Special;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// El ARSENAL del Rey (王之财宝·武具库) — la fuente que MANIFIESTA Armas del Tesoro a la mano, equivalente
/// local del (futuro) módulo Arsenal de FGOCore (DESIGN-GILGAMESH §10). Modelado EXACTO sobre
/// <see cref="TiamatBeast.TiamatCode.Powers.BeastDeck"/>: factoriza el bloque de manifestación para que
/// la firma (Puerta de Babilonia), la Apertura del Tesoro y cualquier otro generador usen el MISMO set
/// de 8 Armas y la misma vía de manifestación (<see cref="ManifestCards.ManifestToHand{T}"/>).
///
/// Las 8 Armas son <see cref="Cards.ITreasureArm"/> + 0⚡ + Exhaust + <c>CardRarity.Event</c> (no
/// drafteables; se esfuman al exhaustarse). Al jugarse, el hook central de <see cref="ArmsPlayedPower"/>
/// cuenta cada una para los riders de tribu.
/// </summary>
public static class TreasureDeck
{
    /// <summary>Las 8 Armas del Tesoro, cada una como un manifestador a la mano. El índice se elige con
    /// el RNG determinista de combate (igual orden que el kit del diseño §5.5).</summary>
    private static readonly Func<Creature, Task>[] Arms =
    [
        c => ManifestCards.ManifestToHand<Merodach>(c),
        c => ManifestCards.ManifestToHand<Caladbolg>(c),
        c => ManifestCards.ManifestToHand<Harpe>(c),
        c => ManifestCards.ManifestToHand<Vajra>(c),
        c => ManifestCards.ManifestToHand<Durandal>(c),
        c => ManifestCards.ManifestToHand<Houtengeki>(c),
        c => ManifestCards.ManifestToHand<Dainsleif>(c),
        c => ManifestCards.ManifestToHand<TreasureCuirass>(c),
    ];

    /// <summary>Cantidad total de Armas distintas en el arsenal.</summary>
    public static int Count => Arms.Length;

    /// <summary>Manifiesta a la mano <paramref name="count"/> Armas elegidas AL AZAR (con repetición), con
    /// su preview animado. Patrón <c>BeastDeck.ManifestAll</c> + el RNG <c>Rng.CombatCardGeneration</c> del
    /// ecosistema (QueensScepter/ClaimCaliburn). No-op si la criatura no está en combate (lo gestiona
    /// ManifestCards).</summary>
    public static async Task ManifestRandom(Creature creature, int count)
    {
        var rng = creature.CombatState?.RunState.Rng.CombatCardGeneration;
        if (rng == null) return;
        for (var i = 0; i < count; i++)
        {
            var index = rng.NextInt(Arms.Length);
            await Arms[index](creature);
        }
    }

    /// <summary>Manifiesta a la mano las 8 Armas del arsenal de golpe (la cosecha del «Tesoro Abierto»
    /// total), en el orden del kit. Espejo de <c>BeastDeck.ManifestAll</c>.</summary>
    public static async Task ManifestAll(Creature creature)
    {
        foreach (var manifest in Arms)
        {
            await manifest(creature);
        }
    }
}
