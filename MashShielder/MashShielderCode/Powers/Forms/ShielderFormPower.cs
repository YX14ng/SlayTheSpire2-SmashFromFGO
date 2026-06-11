namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>Forma Shielder — defensive stance: Block generates NP Charge, first wall each turn is taller.</summary>
public sealed class ShielderFormPower : MashFormPower
{
    protected override bool ShielderPassive => true;

    public override string FramesPath => $"{MainFile.ResPath}/character/mash_frames_base.tres";
}
