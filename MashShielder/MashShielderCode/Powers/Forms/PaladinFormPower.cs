namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>Forma Paladín — Mash's own knighthood: both passives, no penalty. Permanent.</summary>
public sealed class PaladinFormPower : FormPower
{
    protected override bool ShielderPassive => true;

    protected override bool OrtinaxPassive => true;
}
