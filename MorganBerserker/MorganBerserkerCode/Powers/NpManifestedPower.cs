using MegaCrit.Sts2.Core.Entities.Powers;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Marker: the NP gauge crossed 100 and the ult already manifested for this peak.
/// Removed when the gauge drops below 100 (FGOCore GaugeDropped), so refilling
/// manifests the ult again. (Patrón CamelotManifestedPower de Mash.)
/// </summary>
public sealed class NpManifestedPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
