using MegaCrit.Sts2.Core.Entities.Powers;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>Espíritu Pionero de las Estrellas — the first NP card each combat costs no NP Charge.</summary>
public sealed class PioneerOfTheStarsPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public bool Used { get; set; }
}
