using BaseLib.Abstracts;
using MashShielder.MashShielderCode.Extensions;
using Godot;

namespace MashShielder.MashShielderCode.Character;

public class MashShielderRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => MashShielder.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}