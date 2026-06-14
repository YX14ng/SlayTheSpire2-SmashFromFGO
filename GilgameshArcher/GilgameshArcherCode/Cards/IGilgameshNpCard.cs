namespace GilgameshArcher.GilgameshArcherCode.Cards;

/// <summary>
/// Marcador de carta-NP de Gilgamesh: las que consumen TODO el medidor vía
/// <c>NpCharge.ConsumeAllForNpCard</c> (ENUMA ELISH: Desatado, NP Enuma Elish drafteable, Puerta de
/// Babilonia: Andanada del Rey). Lo leen las reliquias/poderes que reaccionan a «la primera carta-NP
/// del turno» o al clímax (Recital de la Creación). Patrón ISiegfriedNpCard / IMashNpCard / IArtoriaNpCard.
/// </summary>
public interface IGilgameshNpCard;
