using MegaCrit.Sts2.Core.Entities.Powers;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// Noble Phantasm gauge (0-100). Gained from cards and passives; spent by NP cards.
/// Stack amount is the current charge. Use <see cref="NpCharge"/> helpers to gain/spend
/// so the 100 cap is always respected.
/// </summary>
public sealed class NpChargePower : FGOCorePower
{
    public const int Max = 100;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
