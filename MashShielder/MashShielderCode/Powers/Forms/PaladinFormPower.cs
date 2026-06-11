namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>Forma Paladín — Mash's own knighthood: both passives, no penalty. Permanent.</summary>
public sealed class PaladinFormPower : MashFormPower
{
    protected override bool ShielderPassive => true;

    protected override bool OrtinaxPassive => true;

    public override bool IsPermanent => true;

    public override string FramesPath => $"{MainFile.ResPath}/character/mash_frames_paladin.tres";
}
