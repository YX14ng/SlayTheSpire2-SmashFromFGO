namespace MashShielder.MashShielderCode.Powers;

using MegaCrit.Sts2.Core.Entities.Powers;

/// <summary>
/// Control de "Embate de Lord Camelot" — limita la carta a 1 disparo por turno (P2 2026-06-25).
/// Persistente (no se auto-quita): guarda un flag <c>_firedThisTurn</c> que se resetea al inicio
/// del turno del jugador (patrón <see cref="MashShielderPower.OnPlayerTurnStartReset"/>, idéntico a
/// WallDoctrinePower). La carta consulta <see cref="CanFire"/> para glow/jugabilidad y llama
/// <see cref="MarkFired"/> al resolver. Evita el bucle de re-jugar el nuke Baluarte→daño.
/// </summary>
public sealed class LordCamelotChargePower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private bool _firedThisTurn;

    public bool CanFire => !_firedThisTurn;

    public void MarkFired()
    {
        _firedThisTurn = true;
        Flash();
    }

    protected override void OnPlayerTurnStartReset() => _firedThisTurn = false;
}
