namespace FGOCore.FGOCoreCode.DragonScales;

/// <summary>
/// A power on the owner that SUPPRESSES the <see cref="DragonScalesPower"/> reduction
/// for the whole turn — EVERY incoming hit passes through unreduced, not just the first
/// (that is <see cref="IDragonScalePiercer"/>'s job). Used by Siegfried's "Espalda
/// Expuesta" card (the risky mirror: he drops his guard to strike harder).
///
/// <see cref="DragonScalesPower.ModifyHpLostBeforeOsty"/> queries every implementer on
/// the owner's powers and skips ALL scale reduction this turn if any returns
/// <c>true</c>. Like the piercer query, this MUST be a pure read (incoming-damage
/// previews such as BetterSpire2 may evaluate it without the hit resolving): hold the
/// suppression as a Single power that self-removes at end of the player's turn, never a
/// per-hit mutation here.
///
/// NOTE: suppression does NOT make hits cheaper for the Linden Leaf NP refund — a
/// suppressed hit was simply not reduced by the scales, so it is "fully reaching" and
/// behaves like a pierced hit, not a reduced one (the relic's P2 ≥1-residual rule only
/// pays out on hits the scales actually shrank).
/// </summary>
public interface ISdDSuppressor
{
    /// <summary>True while the Dragon Scales are suppressed (no per-hit reduction).</summary>
    bool SuppressScales { get; }
}
