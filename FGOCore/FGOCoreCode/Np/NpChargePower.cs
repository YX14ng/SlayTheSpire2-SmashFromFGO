using MegaCrit.Sts2.Core.Entities.Powers;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// Noble Phantasm gauge (0-300, like FGO). Gained from cards and passives; spent by
/// NP cards. Stack amount is the current charge. Use <see cref="NpCharge"/> helpers
/// to gain/spend so the cap is always respected. Ults manifest at
/// <see cref="ManifestThreshold"/> (100), not at the cap.
/// </summary>
public sealed class NpChargePower : FGOCorePower
{
    public const int Max = 300;

    public const int ManifestThreshold = 100;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
