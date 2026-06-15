using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Combat;

/// <summary>
/// Helpers para "manifestar" una carta generada directamente en combate (el patrón de las
/// ultis AUTO-MANIFESTADAS: cruzar 100 de Carga NP crea la carta-ulti en la mano, con su
/// preview animado). Estaba duplicado palabra-por-palabra en los <c>MainFile.cs</c> de
/// Mordred / Gilgamesh / Okita / Oberon (crear con <c>CombatState.CreateCard</c>,
/// <c>CardPileCmd.AddGeneratedCardToCombat</c> + <c>CardCmd.PreviewCardPileAdd</c>).
///
/// ADITIVO: los mods PUEDEN adoptar estos métodos; nada los obliga. La firma sugerida por el
/// encargo —<c>ManifestToHand&lt;T&gt;(Creature)</c>— es el caso común (carta conocida en
/// tiempo de compilación, a la mano). La sobrecarga con <see cref="CardModel"/> cubre a Oberon,
/// que elige la carta según la forma activa y por eso parte de una instancia ya creada.
/// </summary>
public static class ManifestCards
{
    /// <summary>Preview por defecto de las ultis del ecosistema (Mordred/Okita/Oberon usan 1.0f).</summary>
    public const float DefaultPreviewTime = 1.0f;

    /// <summary>
    /// Crea una carta <typeparamref name="T"/> para el jugador dueño de <paramref name="creature"/>
    /// y la mete en la pila indicada (por defecto la mano), con el preview animado.
    /// No hace nada si la criatura no está en combate o no tiene jugador.
    /// </summary>
    public static Task ManifestToHand<T>(Creature creature, float previewTime = DefaultPreviewTime)
        where T : CardModel
        => ManifestToPile<T>(creature, PileType.Hand, previewTime);

    /// <summary>Versión general de <see cref="ManifestToHand{T}"/> hacia una pila cualquiera.</summary>
    public static async Task ManifestToPile<T>(Creature creature, PileType pile, float previewTime = DefaultPreviewTime)
        where T : CardModel
    {
        if (creature.CombatState == null || creature.Player == null) return;
        var card = creature.CombatState.CreateCard<T>(creature.Player);
        await Manifest(creature, card, pile, previewTime);
    }

    /// <summary>
    /// Manifiesta una carta YA CREADA (Oberon la elige según la forma activa antes de llamar).
    /// La carta debe haberse creado con <c>creature.CombatState.CreateCard&lt;...&gt;(creature.Player)</c>.
    /// </summary>
    public static Task ManifestToHand(Creature creature, CardModel card, float previewTime = DefaultPreviewTime)
        => Manifest(creature, card, PileType.Hand, previewTime);

    /// <summary>Versión general hacia una pila cualquiera, para una carta ya creada.</summary>
    public static async Task Manifest(Creature creature, CardModel card, PileType pile, float previewTime = DefaultPreviewTime)
    {
        if (creature.CombatState == null) return;
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(card, pile, addedByPlayer: true), previewTime);
    }
}
