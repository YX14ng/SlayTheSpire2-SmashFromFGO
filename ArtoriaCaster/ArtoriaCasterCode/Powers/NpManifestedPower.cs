using MegaCrit.Sts2.Core.Entities.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Marker for the CURRENT NP peak: the gauge is at/above 100 and "Around Caliburn:
/// Desatado" has already been manifested for THIS peak. Applied by
/// <c>MainFile.TryManifestUlt</c> (subscribed to <c>NpCharge.GaugeFilled</c>) so a single
/// peak only manifests the ult ONCE — re-charging without dropping below 100 does not
/// duplicate it. Removed by <c>MainFile.DisarmManifest</c> (subscribed to
/// <c>NpCharge.GaugeDropped</c>) when the gauge falls below 100, which RE-ARMS the trigger:
/// the next time the gauge crosses 100 it is a new peak and the ult manifests again.
/// So the ult manifests once per PEAK, not once per combat.
/// </summary>
public sealed class NpManifestedPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
