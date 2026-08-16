using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using KagetoraLancer.KagetoraLancerCode.Character;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Extensions;
using MegaCrit.Sts2.Core.HoverTips;

namespace KagetoraLancer.KagetoraLancerCode.Cards;

[Pool(typeof(KagetoraCardPool))]
public abstract class KagetoraCard(
    int cost, CardType type, CardRarity rarity, TargetType target, Precept precept = Precept.None) :
    CustomCardModel(cost, type, rarity, target), IPreceptCard
{
    public Precept Precept { get; protected set; } = precept;

    /// <summary>
    /// Canal 5 de UI (§4.5): ¿esta carta avanzaría la Doctrina si la jugara ahora? Público para que
    /// el texto de la carta ("si esto avanza Pecho, +10★") lea EXACTAMENTE la misma condición que
    /// dora el borde — `AfterCardPlayed` corre después de `OnPlay`, así que durante la resolución
    /// esta predicción sigue siendo verdadera.
    ///
    /// Sin dueño no hay glow: ver <see cref="DoctrineEngine"/>.
    /// </summary>
    public bool WouldAdvanceNow => Precept != Precept.None && DoctrineEngine?.WouldAdvance(Precept) == true;

    /// <summary>
    /// El motor, o null si esta carta no está en una partida (modelo canónico, sin dueño, fuera de
    /// combate). Usalo en TODO lo que el juego consulte fuera del turno: glow, descripciones,
    /// <c>IsPlayable</c>. Dentro de <c>OnPlay</c> el dueño existe siempre.
    ///
    /// La guarda es doble a propósito. `ShouldGlowGold` es un getter PÚBLICO que se consulta fuera
    /// de combate (compendio, pantalla de recompensa, biblioteca de cartas): ahí `Owner` puede ser
    /// null —de ahí el encadenamiento entero con `?.`— y sobre un modelo CANÓNICO el getter de
    /// `CardModel.Owner` ni siquiera devuelve null: abre con `AssertMutable()`
    /// (decompiled/MegaCrit.Sts2.Core.Models/AbstractModel.cs:113) y tira `CanonicalModelException`,
    /// que el `?.` no atrapa. Una excepción acá revienta las 69 cartas de una sola vez.
    /// </summary>
    protected DoctrinePower? DoctrineEngine =>
        IsMutable ? Owner?.Creature?.GetPower<DoctrinePower>() : null;

    /// <summary>
    /// Canal 5 (regla 4.6.5): glow dorado en toda condicional. Condición vacía (sin precepto) = sin
    /// glow. Las subclases con una condicional PROPIA sobrescriben esto y le hacen OR con
    /// <see cref="WouldAdvanceNow"/> — salvo las que no tienen precepto (los dos NP, las Choose*),
    /// donde este término es constantemente falso y su override manda tal cual.
    /// </summary>
    protected override bool ShouldGlowGoldInternal => WouldAdvanceNow;

    /// <summary>
    /// Canal 4 (§4.4): las 69 cartas explican el sistema. El tooltip de la Doctrina se agrega una
    /// sola vez acá, en la base.
    /// OJO: una subclase que sobrescriba `ExtraHoverTips` REEMPLAZA esta lista y pierde el tooltip.
    /// Los tooltips propios de una carta van en <see cref="PreceptHoverTips"/>.
    /// (`CardModel.HoverTips` cierra con `.Distinct()`, así que repetir un tooltip es inofensivo.)
    /// </summary>
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DoctrinePower>(), .. PreceptHoverTips];

    /// <summary>Tooltips propios de la carta, además del de la Doctrina.</summary>
    protected virtual IEnumerable<IHoverTip> PreceptHoverTips => [];

    private string CardPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    private string BigCardPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string CustomPortraitPath => ResourceLoader.Exists(BigCardPortraitPath)
        ? BigCardPortraitPath
        : "res://FGOCore/images/card_portraits/big/card.png";

    public override string PortraitPath => ResourceLoader.Exists(CardPortraitPath)
        ? CardPortraitPath
        : "res://FGOCore/images/card_portraits/card.png";
}
