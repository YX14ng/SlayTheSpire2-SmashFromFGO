using MegaCrit.Sts2.Core.Entities.Powers;

namespace AstolfoRider.AstolfoRiderCode.Powers;

public abstract class AstolfoRelicMarkerPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

public sealed class HippogriffFeatherUsedPower : AstolfoRelicMarkerPower;
public sealed class TrifasRibbonUsedPower : AstolfoRelicMarkerPower;
public sealed class GoldenArgaliaPointUsedPower : AstolfoRelicMarkerPower;
public sealed class DoodledManualUsedPower : AstolfoRelicMarkerPower;
public sealed class ImpossibleScaleUsedPower : AstolfoRelicMarkerPower;
public sealed class BorrowedAchillesShieldUsedPower : AstolfoRelicMarkerPower;
public sealed class AdventureBagChosenPower : AstolfoRelicMarkerPower;
