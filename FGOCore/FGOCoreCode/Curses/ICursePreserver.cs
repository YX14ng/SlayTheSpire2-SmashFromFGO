namespace FGOCore.FGOCoreCode.Curses;

/// <summary>
/// Marker for a power on a PLAYER: while any opponent of the cursed creature has
/// one, Curses no longer decay after dealing their damage (Morgan's Curse of
/// Cernunnos). The 15-per-enemy cap still applies.
/// </summary>
public interface ICursePreserver;
