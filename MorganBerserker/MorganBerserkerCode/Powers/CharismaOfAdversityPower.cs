using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Carisma de la Adversidad (逆境的魅力) — Aesc S2: your attacks deal +Amount damage
/// per missing-HP threshold crossed (any / 25% / 50% / 75% missing).
/// </summary>
public sealed class CharismaOfAdversityPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int ThresholdsCrossed
    {
        get
        {
            var missing = 1.0 - (double)Owner.CurrentHp / (double)Owner.MaxHp;
            if (missing <= 0) return 0;
            var crossed = 1;
            if (missing >= 0.25) crossed++;
            if (missing >= 0.50) crossed++;
            if (missing >= 0.75) crossed++;
            return crossed;
        }
    }

    // ModifyDamageAdditive es DELTA (default 0).
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return ThresholdsCrossed * Amount;
    }
}
