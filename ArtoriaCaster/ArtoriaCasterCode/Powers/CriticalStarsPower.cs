using MegaCrit.Sts2.Core.Entities.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Estrellas Críticas (★ / Critical Stars / 暴击星) — Castoria's resource counter,
/// capped at <see cref="Max"/>. Earned mostly in Caster form; spent by CRITICAL
/// attacks in Berserker/Avalon form (see <see cref="Stars"/>). Deliberately a
/// mod-local power: it must NOT reuse the Regent's vanilla star system (co-op
/// collision risk, documented in DESIGN-ARTORIA.md §3).
/// </summary>
public sealed class CriticalStarsPower : ArtoriaPower
{
    // 10 -> 12 en el re-baseo al entorno Hextech+BetterCharacters (DESIGN-ARTORIA §8.bis).
    public const int Max = 12;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;
}
