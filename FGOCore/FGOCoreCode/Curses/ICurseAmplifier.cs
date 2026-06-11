namespace FGOCore.FGOCoreCode.Curses;

/// <summary>
/// A power that makes its owner's Curse applications stronger (e.g. Morgan's
/// Fairy Queen form or Item Construction: cards that apply Curse apply +1).
/// <see cref="Curses.Apply"/> sums every amplifier on the applier.
/// </summary>
public interface ICurseAmplifier
{
    int ExtraCurse { get; }
}
