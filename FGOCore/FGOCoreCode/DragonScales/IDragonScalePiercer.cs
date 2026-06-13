using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.DragonScales;

/// <summary>
/// A relic/power on the owner that can let an incoming hit IGNORE its
/// <see cref="DragonScalesPower"/> reduction — Siegfried's exposed back (the Linden
/// leaf). <see cref="DragonScalesPower.ModifyHpLostBeforeOsty"/> queries every
/// implementer for each <em>reaching</em> hit (post-Block, amount &gt; 0, powered
/// attack) and skips the scale reduction if any returns <c>true</c>.
///
/// IMPORTANT: this MUST be a pure read — do NOT mutate per-turn state here. The hook
/// can be evaluated by incoming-damage previews (e.g. BetterSpire2) without the hit
/// actually resolving, so consume the once-per-turn allowance in your own
/// <c>AfterDamageReceived</c> (the real damage path), not in this query. See
/// Siegfried's Linden Leaf relic for the canonical implementation.
/// </summary>
public interface IDragonScalePiercer
{
    /// <summary>True if this reaching hit should bypass the Dragon Scales (no reduction).</summary>
    bool ShouldPierceScales(ValueProp props, Creature? dealer);
}
