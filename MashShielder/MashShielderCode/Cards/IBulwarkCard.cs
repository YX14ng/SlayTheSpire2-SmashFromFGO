namespace MashShielder.MashShielderCode.Cards;

/// <summary>
/// Marca una carta cuyo Bloqueo YA viene con Baluarte (usa <c>BlockRetention.GainBulwarkBlock</c>).
/// La pasiva de Shielder la salta para no aplicar stacks dos veces (guard F6, ver
/// <see cref="Powers.Forms.MashFormPower.AfterBlockGained"/>).
/// </summary>
public interface IBulwarkCard;
