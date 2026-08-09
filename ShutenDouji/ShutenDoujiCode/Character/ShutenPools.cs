using BaseLib.Abstracts;
using Godot;

namespace ShutenDouji.ShutenDoujiCode.Character;

public sealed class ShutenCardPool : CustomCardPoolModel,
    STS2RitsuLib.Scaffolding.Characters.IModColorfulPhilosophersCardPool
{
    public override string Title => Shuten.CharacterId;
    public override float H => 0.82f;
    public override float S => 0.62f;
    public override float V => 0.88f;
    public override Color DeckEntryCardColor => Shuten.Color;
    public override bool IsColorless => false;
}

public sealed class ShutenRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Shuten.Color;
}

public sealed class ShutenPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Shuten.Color;
}
