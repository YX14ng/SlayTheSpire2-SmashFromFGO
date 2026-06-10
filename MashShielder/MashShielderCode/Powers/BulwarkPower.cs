using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Baluarte — Block up to this power's amount is not removed at the start of your turn.
/// Granted by cards that give "Bulwark Block". Retention is computed jointly with the
/// Round Table relic via <see cref="BlockRetention"/> so multiple preventers never fight.
/// </summary>
public sealed class BulwarkPower : MashShielderPower
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
}
