using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// Regla de Oro / Avaricia Dorada (黄金律) — amplifies EVERY NP-charge gain this combat by
/// <see cref="Ratio"/> per stack (+50%). Generic: reusable by any servant with an
/// NP-gain-up. Mirrors <c>CritStarsPower</c>'s instance <c>_amplifying</c> guard (NOT a
/// static field on the engine): it reacts to the owner's own <see cref="NpChargePower"/>
/// rising and adds the extra via <c>PowerCmd.ModifyAmount</c> DIRECTLY — not through
/// <see cref="NpCharge.Gain"/> — so it neither re-fires GaugeFilled nor re-amplifies.
///
/// The <c>amount</c> argument is the DELTA of the change (verified: PowerCmd passes the
/// applied amount / offset, not the new total), so this is +50% of each gain, never a
/// compounding amplification. It amplifies only charge another source already produced —
/// it is never itself a source of charge (no AFK battery, P2). Personal: no MP scaling.
/// </summary>
public sealed class GoldenRulePower : FGOCorePower
{
    public const decimal Ratio = 0.5m;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _amplifying;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(power, amount, applier, cardSource);
        if (_amplifying) return;
        if (power is not NpChargePower np || power.Owner != Owner) return;
        if (amount <= 0m) return;

        var extra = (int)System.Math.Floor(amount * Ratio * Amount);
        // Respetar el tope 0-300 del medidor (ModifyAmount crudo lo ignoraría, igual que NpCharge.Gain capea).
        extra = System.Math.Min(extra, NpChargePower.Max - (int)np.Amount);
        if (extra <= 0) return;

        _amplifying = true;
        Flash();
        await PowerCmd.ModifyAmount(np, extra, Owner, cardSource);
        _amplifying = false;
    }
}
