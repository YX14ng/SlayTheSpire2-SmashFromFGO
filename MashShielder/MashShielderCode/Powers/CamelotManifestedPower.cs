using MegaCrit.Sts2.Core.Entities.Powers;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Marker: the NP gauge is at 100 and Lord Camelot already manifested for this peak
/// (the free ult card was generated). Removed when the gauge drops below max, so
/// refilling to 100 manifests the ult again.
/// </summary>
public sealed class CamelotManifestedPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
