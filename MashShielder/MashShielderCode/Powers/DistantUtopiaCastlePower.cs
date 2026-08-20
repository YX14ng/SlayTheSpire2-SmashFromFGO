using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Castillo de la Utopía Lejana — conservás Bloqueo entre turnos hasta la altura del castillo
/// (<see cref="PowerModel.Amount"/>: 40, mejora 60).
///
/// <para>REDESIGN-MASH-V2 §6.3 (parche J1-4). ANTES devolvía <c>decimal.MaxValue</c>: retención
/// INFINITA, o sea la Barricada que, combinada con el ratchet de Baluarte, producía el reporte de
/// Moopamoop. Ahora es un tope declarado — un castillo tiene una altura.</para>
///
/// <para>Parche F1 de la revisión adversarial: este power es un preventer PROPIO que antes NO
/// delegaba en <see cref="BlockRetention.Enforce"/> (sólo hacía <c>Flash()</c>). Como el juego elige
/// UN SOLO preventer y <c>BulwarkPower</c> se re-aplica cada turno (⇒ cae al final del orden de
/// listeners), este power gana la carrera casi siempre: sin la delegación, el tope de 40 NO se
/// aplicaría nunca y seguiríamos reteniendo todo.</para>
///
/// <para>Interacción con Baluarte, declarada: <c>BlockRetention.Cap</c> suma los stacks de Baluarte
/// al MÁXIMO de las fuentes, así que el techo real del turno es 40 + lo que hayas Baluarteado ESE
/// turno. No hay ratchet (el Baluarte se gasta al retener), así que el techo efectivo queda acotado
/// en ~40-60 en vez de crecer sin fin.</para>
/// </summary>
public sealed class DistantUtopiaCastlePower : MashShielderPower, IBlockRetentionSource
{
    public const int BaseHeight = 40;
    public const int UpgradedHeight = 60;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public decimal RetentionCap(Creature creature) => creature == Owner ? Amount : 0m;

    public override bool ShouldClearBlock(Creature creature) => creature != Owner;

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != Owner) return;
        await BlockRetention.Enforce(creature);
        Flash();
    }
}
