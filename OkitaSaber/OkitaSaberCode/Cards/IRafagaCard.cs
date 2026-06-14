namespace OkitaSaber.OkitaSaberCode.Cards;

/// <summary>
/// Marca una carta como RÁFAGA (缩地 / Dash) — la keyword propia de Okita. Una Ráfaga
/// cuesta su ⚡ normal MÁS un segundo coste en *Aliento* (吐息, 1-3), igual que la doble
/// moneda ⚡+★ del Regent vanilla. El coste se paga en <see cref="Cards.Rafaga.Pay"/> dentro
/// del OnPlay; <see cref="RafagaCost"/> es el coste de Aliento que el motor (Paso Constante,
/// Hasta el Final, Flor del Bakumatsu) lee para descontar o sustituir.
///
/// No es un CardKeyword vanilla (la doble-moneda no tiene API de display verificada en el
/// ecosistema): la Ráfaga se lee por esta interfaz y el coste se enseña en el texto de la
/// carta (igual que las cartas-NP enseñan "consume TODA la Carga"). La jugabilidad se gatea
/// con <c>IsPlayable</c> (Aliento suficiente) y el glow dorado con <c>ShouldGlowGoldInternal</c>.
/// </summary>
public interface IRafagaCard
{
    /// <summary>Puntos de *Aliento que cuesta esta Ráfaga (1-3).</summary>
    int RafagaCost { get; }
}
