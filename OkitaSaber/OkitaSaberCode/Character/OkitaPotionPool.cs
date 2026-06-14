using BaseLib.Abstracts;
using Godot;
using OkitaSaber.OkitaSaberCode.Extensions;

namespace OkitaSaber.OkitaSaberCode.Character;

public class OkitaPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Okita.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
