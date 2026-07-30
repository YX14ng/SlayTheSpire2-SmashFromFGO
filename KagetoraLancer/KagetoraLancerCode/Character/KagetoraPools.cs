using BaseLib.Abstracts;
using Godot;
using KagetoraLancer.KagetoraLancerCode.Extensions;

namespace KagetoraLancer.KagetoraLancerCode.Character;

public sealed class KagetoraCardPool : CustomCardPoolModel
{
    public override string Title => Kagetora.CharacterId;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
    public override float H => 0.55f;
    public override float S => 0.45f;
    public override float V => 1f;
    public override Color DeckEntryCardColor => Kagetora.Color;
    public override bool IsColorless => false;
}

public sealed class KagetoraRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Kagetora.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}

public sealed class KagetoraPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Kagetora.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
