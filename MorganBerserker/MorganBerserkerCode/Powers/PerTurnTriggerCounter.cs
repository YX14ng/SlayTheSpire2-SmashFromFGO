using MegaCrit.Sts2.Core.Combat;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Contador de activaciones por turno con tope, reutilizado por
/// <see cref="MadnessEnhancementPower"/> (NP por perder HP, cap = Amount) y por
/// <see cref="Relics.QueensScepter"/> (Estrellas por perder HP, cap = 3). Encapsula
/// SOLO el conteo + el reset al inicio del turno del jugador + el chequeo de tope;
/// cada consumidor mantiene sus propios guards (turno del jugador, exclusión del
/// tick de FaeBloodPact, etc.) y pasa su propio tope, que puede ser dinámico (Amount).
/// </summary>
internal sealed class PerTurnTriggerCounter
{
    private int _count;

    /// <summary>Resetea el contador al comenzar el turno del jugador (llamar desde AfterSideTurnStart).</summary>
    public void OnSideTurnStart(CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            _count = 0;
        }
    }

    /// <summary>
    /// Intenta consumir una activación: devuelve <c>false</c> si ya se alcanzó <paramref name="max"/>
    /// este turno; si hay cupo, incrementa y devuelve <c>true</c>.
    /// </summary>
    public bool TryConsume(int max)
    {
        if (_count >= max)
        {
            return false;
        }
        _count++;
        return true;
    }
}
