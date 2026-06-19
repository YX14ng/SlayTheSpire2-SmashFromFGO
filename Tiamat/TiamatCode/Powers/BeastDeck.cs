using FGOCore.FGOCoreCode.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using TiamatBeast.TiamatCode.Cards.Special;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// El MAZO ESPECIAL Bestia (rediseño dos-pozas, ver docs/REDESIGN-TIAMAT.md). Al abrir la ventana
/// de NP <c>Nammu Dur-an-ki</c>, las 7 cartas Bestia se MANIFIESTAN a la mano de golpe (con su
/// preview animado). Son <see cref="Cards.IBeastEphemeral"/> + Exhaust + <c>CardRarity.Special</c>:
/// no son drafteables y NUNCA entran al mazo persistente — al cerrarse la ventana
/// (<see cref="TiamatBeastWindowPower.PurgeEphemeral"/>) las que queden se esfuman.
///
/// Factoriza el bloque de manifestación para que tanto la carta-apertura del NP como cualquier
/// efecto futuro (re-manifestar dentro de la ventana) usen el MISMO conjunto y orden.
/// </summary>
public static class BeastDeck
{
    /// <summary>Manifiesta a la mano las 7 cartas del mazo Bestia, en orden de lectura del kit.
    /// No-op si la criatura no está en combate o no tiene jugador (lo gestiona ManifestCards).</summary>
    public static async Task ManifestAll(Creature creature)
    {
        await ManifestCards.ManifestToHand<ChaosTideDeluge>(creature);
        await ManifestCards.ManifestToHand<SpawnLahmuBeast>(creature);
        await ManifestCards.ManifestToHand<FeedLahmuBeast>(creature);
        await ManifestCards.ManifestToHand<DevourTheChildren>(creature);
        await ManifestCards.ManifestToHand<LahmuTide>(creature);
        await ManifestCards.ManifestToHand<BrokenHornBeast>(creature);
        await ManifestCards.ManifestToHand<PlumaDeLaBestia>(creature);
    }
}
