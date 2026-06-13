using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Extensions;

/// <summary>
/// Helpers del mod. <see cref="RemoveAllDebuffs"/> centraliza el cleanse de las DOS únicas cartas que la
/// regla P8 permite (Retirada Estratégica = Exhaust; Corona del Sin Par = max 1). Quita SOLO powers Debuff
/// efectivos del propio jugador (filtro signado TypeForCurrentAmount, patrón Misery) — NUNCA instala Artifact
/// ni prevención pasiva. Los recursos de Siegfried (SdD/NP/Overcharge) son Buff → intactos; Maldición es
/// Debuff → se limpia (deseado).
/// </summary>
public static class SiegfriedExtensions
{
    public static async Task RemoveAllDebuffs(Creature target, int max = int.MaxValue)
    {
        var debuffs = target.Powers.Where(p => p.TypeForCurrentAmount == PowerType.Debuff).Take(max).ToList();
        foreach (var power in debuffs)
        {
            await PowerCmd.Remove(power);
        }
    }
}
