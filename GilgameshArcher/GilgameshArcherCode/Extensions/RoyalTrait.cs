using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;

namespace GilgameshArcher.GilgameshArcherCode.Extensions;

/// <summary>
/// Resuelve "lo divino" (DESIGN-GILGAMESH §6): el superefectivo de Enuma Elish, la Cadena del Cielo
/// y el Golpe de Ea muerden más a lo que TRASCIENDE a lo humano. Sin sistema de trait vanilla
/// (CONFIRMADO en Siegfried: ningún enum de raza), el gloss auditable del diseño es "Élites y Jefes
/// son los seres de rango divino del Spire". Lectura PURA (preview-safe), nunca muta estado —
/// espejo 1:1 de <c>DragonTrait.IsDragon</c> de Siegfried. Contra "lo meramente humano" (Monster
/// común, o un Monster lanzado desde un Evento que reporta otro RoomType) NO da bonus: el anti-bonus
/// es fiel al NP real Y es balance (§3: el rider que escala vs divino DEBE poder fallar).
/// </summary>
public static class RoyalTrait
{
    public static bool IsDivine(Creature? enemy)
    {
        if (enemy == null || enemy.IsPlayer) return false;
        return enemy.CombatState?.RunState.CurrentRoom?.RoomType is RoomType.Elite or RoomType.Boss;
    }
}
