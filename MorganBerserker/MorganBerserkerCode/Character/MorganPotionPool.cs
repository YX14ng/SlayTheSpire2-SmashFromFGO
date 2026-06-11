using BaseLib.Abstracts;
using Godot;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Character;

public class MorganPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => MorganBerserker.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
