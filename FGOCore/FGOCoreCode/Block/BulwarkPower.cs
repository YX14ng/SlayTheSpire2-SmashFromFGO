using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Block;

/// <summary>
/// Baluarte — Block up to this power's amount survives ONE turn boundary; then the Bulwark is
/// spent and the wall has to be raised again. Granted by cards that give "Bulwark Block".
/// Retention is computed jointly with any <see cref="IBlockRetentionSource"/> via
/// <see cref="BlockRetention"/> so multiple preventers never fight.
///
/// <para>REDESIGN-MASH-V2 §3 CANDADO 1 (reporte de Steam 2026-08-20, Moopamoop: «trivially easy to
/// build up absurd amounts of block and become practically invincible»). ANTES este power NO decaía
/// en ninguna parte y <see cref="BlockRetention.Cap"/> SUMA los stacks ⇒ cada punto de Bloqueo con
/// Baluarte subía el techo de retención para todo el combate, o sea Barricade vendida en rareza
/// COMÚN, y el muro crecía solo y sin tope. Ahora el Baluarte se GASTA al retener.</para>
///
/// <para>Por qué el reset vive acá y no en <see cref="BlockRetention.Enforce"/> (parche F2 de la
/// revisión adversarial): el juego elige UN SOLO preventer de limpieza de Bloqueo
/// (<c>Hook.ShouldClearBlock</c> devuelve el PRIMERO que dice que no), y hay preventers propios que
/// no delegan en <c>Enforce</c> — con el agravante de que re-aplicar este power lo manda al final
/// del orden de listeners, así que perdería la carrera sistemáticamente. <c>AfterBlockCleared</c>
/// (<c>CombatManager</c>) corre para CADA criatura que empieza turno, INCONDICIONALMENTE, después
/// de la fase de clear/prevención: gane quien gane la carrera y haya o no Bloqueo en pie. Es el
/// mismo hook del vanilla <c>BlockNextTurnPower</c>.</para>
/// </summary>
public sealed class BulwarkPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override bool ShouldClearBlock(Creature creature) => creature != Owner;

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != Owner) return;
        await BlockRetention.Enforce(creature);
        Flash();
    }

    /// <summary>
    /// El Baluarte se GASTA: corre después de la fase de clear/prevención del inicio de turno, para
    /// toda criatura que empieza turno y sin importar qué preventer ganó, así que el trim ya se
    /// aplicó (o la retención era infinita por otra vía) y estos stacks ya cumplieron su función.
    /// </summary>
    public override async Task AfterBlockCleared(Creature creature)
    {
        if (creature != Owner) return;
        await PowerCmd.Remove(this);
    }
}
