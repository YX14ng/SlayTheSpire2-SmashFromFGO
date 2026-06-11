namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// A relic that stores the character's NP level (1-5, like FGO) and the dupe-roll
/// pity counter for the run. Implement on a relic with [SavedProperty] backing so
/// both survive save/continue. <see cref="NpLevels"/> reads and writes it.
/// </summary>
public interface INpLevelStore
{
    int NpLevel { get; set; }

    int DupePity { get; set; }
}
