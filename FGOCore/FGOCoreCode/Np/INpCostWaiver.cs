namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// A power granting a free NP card (e.g. Mash's Pioneer of the Stars: the first NP
/// card each combat costs no charge). Implement on a PowerModel; <see cref="NpCharge"/>
/// consumes the waiver instead of spending the gauge, without lowering the Overcharge tier.
/// </summary>
public interface INpCostWaiver
{
    bool Used { get; set; }
}
