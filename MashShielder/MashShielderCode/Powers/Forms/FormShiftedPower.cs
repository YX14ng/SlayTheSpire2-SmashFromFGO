using MegaCrit.Sts2.Core.Entities.Powers;

namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>Marker: Mash changed form this combat (enables Golpe de Vanguardia and friends).</summary>
public sealed class FormShiftedPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
