using MegaCrit.Sts2.Core.Entities.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Marker: the NP gauge crossed 100 and the ult already manifested for this peak.
/// Removed when the gauge drops below 100 (FGOCore GaugeDropped), so refilling
/// manifests the ult again. (Patrón de Mash/Morgan.)
/// </summary>
public sealed class NpManifestedPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
