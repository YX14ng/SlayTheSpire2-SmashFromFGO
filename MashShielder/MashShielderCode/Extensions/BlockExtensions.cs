using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace MashShielder.MashShielderCode.Extensions;

/// <summary>
/// Helpers locales de modularización (AUDIT-2026-06-15): el patrón "consumir / leer el Bloqueo
/// propio como munición" estaba disperso en varias cartas (BunkerBolt, Crush, OrtinaxMaintenance,
/// SharpenedEdge). Centraliza el cálculo (cap + guard de &gt;0) en un único lugar; comportamiento
/// idéntico, sólo menos duplicación.
/// </summary>
public static class BlockExtensions
{
    /// <summary>
    /// Lee el Bloqueo actual capado a <paramref name="cap"/> SIN consumirlo
    /// (equivale a <c>Math.Min(creature.Block, cap)</c>). Usado para el bono de daño escalado.
    /// </summary>
    public static int BlockCappedAt(this Creature creature, int cap) => Math.Min(creature.Block, cap);

    /// <summary>
    /// Consume TODO el Bloqueo de la criatura y devuelve cuánto consumió (0 si no tenía).
    /// Sólo emite <see cref="CreatureCmd.LoseBlock"/> si había Bloqueo (&gt; 0).
    /// </summary>
    public static async Task<int> ConsumeAllBlock(this Creature creature)
    {
        var block = creature.Block;
        if (block > 0)
        {
            await CreatureCmd.LoseBlock(creature, block);
        }
        return block;
    }

    /// <summary>
    /// Consume hasta <paramref name="cap"/> de Bloqueo y devuelve cuánto consumió (0 si no tenía).
    /// Sólo emite <see cref="CreatureCmd.LoseBlock"/> si el monto a consumir es &gt; 0.
    /// </summary>
    public static async Task<int> ConsumeBlockUpTo(this Creature creature, int cap)
    {
        var consumed = creature.BlockCappedAt(cap);
        if (consumed > 0)
        {
            await CreatureCmd.LoseBlock(creature, consumed);
        }
        return consumed;
    }
}
