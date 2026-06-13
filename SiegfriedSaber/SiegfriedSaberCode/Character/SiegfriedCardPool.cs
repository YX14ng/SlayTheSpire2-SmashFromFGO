using BaseLib.Abstracts;
using Godot;
using SiegfriedSaber.SiegfriedSaberCode.Extensions;

namespace SiegfriedSaber.SiegfriedSaberCode.Character;

public class SiegfriedCardPool : CustomCardPoolModel
{
    public override string Title => Siegfried.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override float H => 1f;
    public override float S => 1f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;
}
