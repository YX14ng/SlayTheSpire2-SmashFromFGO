using MegaCrit.Sts2.Core.Entities.Creatures;

namespace FGOCore.FGOCoreCode.Block;

/// <summary>
/// Something that lets a creature retain Block between turns (a relic or a power).
/// <see cref="BlockRetention.Cap"/> takes the MAX over all sources (they don't stack
/// with each other) and adds Bulwark stacks on top. Return decimal.MaxValue for
/// "retain everything" effects.
/// <para>CONTRATO (audit 2026-07-04): esta interfaz alimenta el CÁLCULO del cap, pero el juego solo
/// pregunta <c>ShouldClearBlock</c> para decidir si limpia el Bloqueo — alguien tiene que responder
/// false. Las FORMAS que la implementan ya están cubiertas (la base <see cref="Forms.FormPower"/>
/// responde por ellas y delega en <see cref="BlockRetention.Enforce"/>). Una RELIQUIA que la
/// implemente DEBE además overridear <c>ShouldClearBlock</c> (false para su dueño con cap &gt; 0) y
/// <c>AfterPreventingBlockClear</c> → <c>BlockRetention.Enforce</c>, como la Sturdy Clamp vanilla —
/// ver <c>DragonScaleAegis</c> (Siegfried). Sin eso, la retención de la reliquia solo funciona si
/// casualmente hay un <see cref="BulwarkPower"/> presente.</para>
/// </summary>
public interface IBlockRetentionSource
{
    decimal RetentionCap(Creature creature);
}
