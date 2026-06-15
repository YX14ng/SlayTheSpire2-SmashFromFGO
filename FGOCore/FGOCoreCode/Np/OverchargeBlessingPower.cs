using MegaCrit.Sts2.Core.Entities.Powers;
using FGOCore.FGOCoreCode.Cleanse;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// Bendición de Rhongomyniad (伦戈米尼亚德的祝福) — the "Overcharge +1" of Morgan's
/// real NP: your next NP card adds +10 per stack to its Overcharge tier. Consumed
/// automatically by <see cref="NpCharge.ConsumeAllForNpCard"/>.
/// </summary>
public sealed class OverchargeBlessingPower : FGOCorePower, IResourcePower
{
    public const int TierPerStack = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
