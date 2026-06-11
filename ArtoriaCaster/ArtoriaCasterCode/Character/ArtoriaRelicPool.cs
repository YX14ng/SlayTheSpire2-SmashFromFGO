using BaseLib.Abstracts;
using Godot;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;

namespace ArtoriaCaster.ArtoriaCasterCode.Character;

public class ArtoriaRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => ArtoriaCaster.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
