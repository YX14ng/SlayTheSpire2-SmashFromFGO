using BaseLib.Abstracts;
using Godot;
using GilgameshArcher.GilgameshArcherCode.Extensions;

namespace GilgameshArcher.GilgameshArcherCode.Character;

public class GilgameshPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Gilgamesh.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
