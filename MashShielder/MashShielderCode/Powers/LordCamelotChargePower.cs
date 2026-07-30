namespace MashShielder.MashShielderCode.Powers;

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

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

    public bool CanFire => FgoCombatState.GetTurn(Owner, 3) == 0;

    public async Task MarkFired(PlayerChoiceContext context, CardModel source)
    {
        await FgoCombatState.SetTurn(context, Owner, 3, 1, source);
        Flash();
    }
}
