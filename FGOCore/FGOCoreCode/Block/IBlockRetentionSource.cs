using MegaCrit.Sts2.Core.Entities.Creatures;

namespace FGOCore.FGOCoreCode.Block;

/// <summary>
/// Something that lets a creature retain Block between turns (a relic or a power).
/// <see cref="BlockRetention.Cap"/> takes the MAX over all sources (they don't stack
/// with each other) and adds Bulwark stacks on top. Return decimal.MaxValue for
/// "retain everything" effects.
/// </summary>
public interface IBlockRetentionSource
{
    decimal RetentionCap(Creature creature);
}
