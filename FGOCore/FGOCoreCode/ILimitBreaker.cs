namespace FGOCore.FGOCoreCode;

/// <summary>
/// A relic that breaks FGO-system caps while owned (the Holy Grail). Bond levels
/// beyond 10 and NP levels beyond 5 are unlocked by the sum over all owned breakers.
/// </summary>
public interface ILimitBreaker
{
    int ExtraBondLevels { get; }

    int ExtraNpLevels { get; }
}
