using BaseLib.Abstracts;
using Godot;

namespace AstolfoRider.AstolfoRiderCode.Character;

public sealed class AstolfoCardPool : CustomCardPoolModel
{
    public override string Title => Astolfo.CharacterId;
    public override float H => 0.92f;
    public override float S => 0.48f;
    public override float V => 1f;
    public override Color DeckEntryCardColor => Astolfo.Color;
    public override bool IsColorless => false;
}

public sealed class AstolfoRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Astolfo.Color;
}

public sealed class AstolfoPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Astolfo.Color;
}
