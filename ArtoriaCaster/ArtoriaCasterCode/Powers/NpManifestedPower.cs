using MegaCrit.Sts2.Core.Entities.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Marker: the NP gauge crossed 100 and the "Around Caliburn" window already opened
/// THIS COMBAT. It is NOT removed when the gauge drops below 100 — the NP cards always
/// drain the gauge, so re-charging the same combat must NOT re-grant the support package
/// (estrellas + Anti-Purga + energía + robo). Combat-scoped (cleared between combats),
/// so the window opens exactly once per combat.
/// </summary>
public sealed class NpManifestedPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
