using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace MordredSaber.MordredSaberCode.Extensions;

/// <summary>
/// Helpers compartidos del kit de Mordred. NO inventan APIs nuevas: envuelven patrones ya
/// usados en el ecosistema (el check de RoomType de GuardiansExecution, el cleanse de
/// ArtoriaCard.RemoveOwnDebuffs) para que las cartas anti-autoridad y el casco (Secreto de
/// Cuna EX) los citen sin duplicar lógica.
/// </summary>
public static class MordredExtensions
{
    /// <summary>
    /// El rider temático «vs Élites/Jefes» (la autoridad = el trait [Arthur] del special real,
    /// DESIGN-MORDRED §3.4). Lectura pura del tipo de encuentro; las cartas con glow consultan
    /// esto en su condicional y en <c>ShouldGlowGoldInternal</c>.
    /// </summary>
    public static bool VersusAuthority(this Creature creature) =>
        creature.CombatState?.Encounter?.RoomType is RoomType.Elite or RoomType.Boss;

    /// <summary>
    /// Remueve TODOS los debuffs del owner (el cleanse del casco: «Secreto de Cuna EX» y el
    /// Amuleto de Resistencia Mágica B). Espejo de ArtoriaCard.RemoveOwnDebuffs — el ÚNICO
    /// vector de cleanse permitido del pool (regla negativa §2), por eso vive centralizado.
    /// </summary>
    public static async Task RemoveAllDebuffs(this Creature creature)
    {
        foreach (var debuff in creature.GetPowerInstances<PowerModel>().Where(p => p.Type == PowerType.Debuff).ToList())
        {
            await PowerCmd.Remove(debuff);
        }
    }
}
