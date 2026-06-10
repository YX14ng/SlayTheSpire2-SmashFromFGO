namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>Forma Ortinax — offensive stance: attacks spend Block as ammunition (Bunker Bolt).</summary>
public sealed class OrtinaxFormPower : FormPower
{
    protected override bool OrtinaxPassive => true;

    protected override bool DefensePenalty => true;
}
