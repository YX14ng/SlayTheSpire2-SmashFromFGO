using BaseLib.Abstracts;
using Godot;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Character;

public class MordredPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Mordred.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
