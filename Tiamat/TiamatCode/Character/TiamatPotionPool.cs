using BaseLib.Abstracts;
using Godot;
using TiamatBeast.TiamatCode.Extensions;

namespace TiamatBeast.TiamatCode.Character;

public class TiamatPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Tiamat.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
