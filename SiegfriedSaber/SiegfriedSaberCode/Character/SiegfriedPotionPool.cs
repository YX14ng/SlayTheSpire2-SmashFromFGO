using BaseLib.Abstracts;
using Godot;
using SiegfriedSaber.SiegfriedSaberCode.Extensions;

namespace SiegfriedSaber.SiegfriedSaberCode.Character;

public class SiegfriedPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Siegfried.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
