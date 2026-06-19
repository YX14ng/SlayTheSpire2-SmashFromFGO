namespace TiamatBeast.TiamatCode.Cards;

/// <summary>
/// Marcador: carta del MAZO ESPECIAL Bestia, manifestada en la mano al abrir la ventana de NP
/// ("Nammu Dur-an-ki"). NUNCA entra al mazo persistente (Lily): cuando la ventana se cierra
/// — el Counter de <see cref="Powers.TiamatBeastWindowPower"/> llega a 0, o por reversión de
/// forma, o por fin de combate/muerte — todas las cartas con este marcador que queden en
/// mano/robo/descarte se quitan de combate. Implementado por las 7 cartas del beastSpecialDeck.
/// </summary>
public interface IBeastEphemeral;
