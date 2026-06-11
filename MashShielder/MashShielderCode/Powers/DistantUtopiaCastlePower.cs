using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Castillo de la Utopía Lejana — ALL your Block persists between turns.
/// BlockRetention.Cap returns infinite while this is active (IBlockRetentionSource).
/// </summary>
public sealed class DistantUtopiaCastlePower : MashShielderPower, IBlockRetentionSource
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public decimal RetentionCap(Creature creature) => decimal.MaxValue;

    public override bool ShouldClearBlock(Creature creature) => creature != Owner;

    public override Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this == preventer && creature == Owner) Flash();
        return Task.CompletedTask;
    }
}
