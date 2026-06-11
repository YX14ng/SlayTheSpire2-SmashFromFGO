using BaseLib.Abstracts;
using Godot;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Character;

public class MorganCardPool : CustomCardPoolModel
{
    public override string Title => MorganBerserker.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override float H => 1f;
    public override float S => 1f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;
}
