using MegaCrit.Sts2.Core.Entities.Powers;

namespace FGOCore.FGOCoreCode.Forms;

/// <summary>Marker: the owner changed form this combat (enables form-synergy cards).</summary>
public sealed class FormShiftedPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
