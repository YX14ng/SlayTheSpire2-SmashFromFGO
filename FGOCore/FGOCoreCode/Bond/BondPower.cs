using MegaCrit.Sts2.Core.Entities.Powers;

namespace FGOCore.FGOCoreCode.Bond;

/// <summary>
/// 好感度 — display power: stacks mirror the character's current Bond level for the run.
/// All the actual bonuses are applied by the character's <see cref="BondRelic"/> at combat start.
/// </summary>
public sealed class BondPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
