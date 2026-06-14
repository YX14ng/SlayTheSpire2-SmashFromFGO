using BaseLib.Abstracts;
using Godot;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Character;

public class OberonPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Oberon.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
