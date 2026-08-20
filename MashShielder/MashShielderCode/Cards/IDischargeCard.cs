namespace MashShielder.MashShielderCode.Cards;

/// <summary>
/// Marca una carta que **Descarga** (REDESIGN-MASH-V2 §3 CANDADO 2): consume el Bloqueo propio y lo
/// convierte en su efecto.
///
/// <para>Sirve para dos cosas: (a) el glow dorado condicional (sólo con Bloqueo &gt; 0) y (b) apagar
/// la pasiva de Ortinax en estas cartas. La pasiva come hasta 5 de Bloqueo en <c>BeforeCardPlayed</c>,
/// ANTES del <c>OnPlay</c>: sobre una carta que ya se lleva todo el muro sería cobrar dos veces, y el
/// reembolso de <c>AfterCardPlayed</c> devolvería 5 DESPUÉS de haberlo vaciado (o sea "quedás desnuda"
/// te dejaba con 5). Riesgo 4 de la revisión adversarial.</para>
/// </summary>
public interface IDischargeCard;
