namespace GilgameshArcher.GilgameshArcherCode.Cards;

/// <summary>
/// Marcador de las 8 Armas del Tesoro (王之财宝·武具) generadas por la Puerta de Babilonia. NO toca
/// FGOCore (el módulo Arsenal aún no existe — checklist §10); hasta entonces el «trait de tribu» se
/// lee con <c>Hand.Cards.OfType&lt;ITreasureArm&gt;()</c>, igual que Okita lee sus *Tos por tipo.
///
/// Lo usa <see cref="Special.EnumaElishUnleashed"/> (FIX HOMOGENEIZACIÓN P2): al disparar Enuma,
/// las Armas que queden en la mano se EXHAUSTAN como Sobrecarga extra (cada una +1 al bonus
/// anti-divino) — el gasto del NP depende de la TRIBU de Gil, no solo de la carga genérica.
/// </summary>
public interface ITreasureArm;
