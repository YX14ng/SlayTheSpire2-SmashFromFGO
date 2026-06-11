namespace FGOCore.FGOCoreCode;

/// <summary>
/// A relic that improves how much HP <see cref="GutsPower"/> leaves you at
/// (e.g. Habetrot's Thread: survive at 10 HP instead of 1).
/// </summary>
public interface IGutsFloorBooster
{
    int ImproveFloor(int baseFloor);
}
