using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace FGOCore.FGOCoreCode.DragonScales;

/// <summary>
/// A power/relic on the owner that wants to REACT when a hit actually pierces the
/// <see cref="DragonScalesPower"/> (the exposed back — the first reaching hit each turn
/// that ignores the scales). This is the broadcast counterpart of
/// <see cref="IDragonScalePiercer"/>: the piercer DECIDES a pierce should happen
/// (a pure read, preview-safe), and the owner that grants the pierce (Siegfried's Linden
/// Leaf relic) fires this listener exactly ONCE, on the real damage path, the moment it
/// consumes its 1/turn pierce allowance.
///
/// IMPORTANT (anti-degeneration): the host MUST broadcast this from the real damage path
/// (its <c>AfterDamageReceived</c>), never from the preview-safe <see cref="IDragonScalePiercer"/>
/// query — otherwise a damage preview (e.g. BetterSpire2) would double-fire it. Because a
/// pierce is structurally at most once per turn (the host's own per-turn flag), any listener
/// reacting here is capped at one trigger per turn for free — no per-frequency battery.
/// See <see cref="DragonScalesPierce.Broadcast"/> for the canonical fan-out.
/// </summary>
public interface IDragonScalePierceListener
{
    /// <summary>Called once, on the real damage path, when a reaching hit pierced the scales.</summary>
    Task OnScalesPierced(PlayerChoiceContext choiceContext);
}
