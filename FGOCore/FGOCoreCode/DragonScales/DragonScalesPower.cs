using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.DragonScales;

/// <summary>
/// Sangre de Dragón / Dragonblood (龙血护甲) — Siegfried's scales. Reduces EACH
/// incoming attack by this power's amount, applied AFTER Block and BEFORE Guts. It is
/// per-hit damage reduction, NOT Block: it is never spent and persists between turns
/// (it is his skin). A hit may be reduced all the way to 0 — that is fine, but the
/// armour then worked "for free" (the Linden Leaf relic only refunds NP on a reduced
/// hit that still deals ≥1, enforced relic-side, P2).
///
/// The "first reaching hit each turn ignores the scales" weakness lives in the relic
/// (the Linden leaf), surfaced here via <see cref="IDragonScalePiercer"/> so FGOCore
/// never references the character mod. Only real attacks are reduced
/// (<see cref="ValuePropExtensions.IsPoweredAttack"/> — Curse, Poison and HP-cost
/// self-damage are not "golpes" and pass through).
///
/// The ModifyHpLost* hooks return the ABSOLUTE resulting amount (not a delta).
/// </summary>
public sealed class DragonScalesPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // Personal defensive layer — like Block, it does NOT scale with player count.
    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return amount;
        // Fully blocked / nothing got through: the scales have nothing to absorb and the
        // hit never "reaches" you, so it must not consume the Linden-leaf pierce.
        if (amount <= 0m) return amount;
        // Only blows (powered attacks). Curse/Poison/HP-cost are Unpowered and ignore the scales.
        if (!props.IsPoweredAttack()) return amount;

        if (ShouldPierce(props, dealer)) return amount; // exposed back — scales bypassed

        Flash();
        return System.Math.Max(amount - base.Amount, 0m);
    }

    private bool ShouldPierce(ValueProp props, Creature? dealer)
    {
        var relics = Owner.Player?.Relics;
        if (relics == null) return false;
        foreach (var relic in relics)
        {
            if (relic is IDragonScalePiercer piercer && piercer.ShouldPierceScales(props, dealer)) return true;
        }
        return false;
    }
}
