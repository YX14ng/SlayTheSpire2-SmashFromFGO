namespace OberonPretender.OberonPretenderCode.Powers.Forms;

/// <summary>
/// EL PRINCIPE DEL INVIERNO (The Prince of Winter) -- forma defensiva (modelo 2800110,
/// capa blanca de plumas). Pasiva (DESIGN-OBERON 5):
/// Al final de tu turno tu Deuda NO se cobra; en su lugar gana +1 de interes (bola de nieve).
///  Tus payoffs de Invierno dan Bloqueo.
///
/// La supresion del cobro + el interes viven en <see cref="DebtPower.BeforeTurnEnd"/> (que consulta
/// esta forma): la logica de Deuda esta centralizada en un solo punto, esta forma es el FLAG que la
/// vira. Los riders en Invierno: +X de las cartas leen <c>HasPower<WinterPrincePower>()</c>
/// directo (Abrigo de Plumas, Alas de Libelula, Cuento del Invierno, Negociacion Feerica). La estafa
/// larga: retener TODO el medidor 1-2 turnos para reventar 100/200 sin impuesto, con la bola
/// esperandote a la salida.
/// </summary>
public sealed class WinterPrincePower : OberonFormPower
{
    public override string? FramesPath => $"{MainFile.ResPath}/character/oberon_frames_winter.tres";
}
