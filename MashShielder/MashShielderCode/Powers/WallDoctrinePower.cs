using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>Doctrina del Muro — the first Block card each turn grants this much NP Charge.</summary>
public sealed class WallDoctrinePower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _usedThisTurn;

    protected override void OnPlayerTurnStartReset() => _usedThisTurn = false;

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (creature != Owner || cardSource == null || _usedThisTurn) return;
        _usedThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, Amount, null);
    }
}
