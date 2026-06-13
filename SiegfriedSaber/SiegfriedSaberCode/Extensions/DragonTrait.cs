using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;

namespace SiegfriedSaber.SiegfriedSaberCode.Extensions;

/// <summary>
/// Resuelve "[Dragón]" sin sistema de trait vanilla (CONFIRMADO: ningún monstruo dragón ni enum de
/// raza/tribu en el juego). Gloss auditable del diseño (§7): "los dragones del Spire" = Élites y Jefes.
/// Lectura PURA (preview-safe), nunca muta estado. Una pelea Monster lanzada desde un Evento reporta
/// RoomType distinto → no da bonus (conservador, nunca sobre-paga; el rider de tribu DEBE poder fallar, §3).
/// </summary>
public static class DragonTrait
{
    public static bool IsDragon(Creature? enemy)
    {
        if (enemy == null || enemy.IsPlayer) return false;
        return enemy.CombatState?.RunState.CurrentRoom?.RoomType is RoomType.Elite or RoomType.Boss;
    }
}
