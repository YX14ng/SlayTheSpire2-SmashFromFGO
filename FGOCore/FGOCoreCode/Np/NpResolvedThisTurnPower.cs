using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// P5 marker — "an NP card has already resolved this turn." Hidden, Single. Set inside
/// <see cref="NpCharge.ConsumeAllForNpCard"/> (the single chokepoint every NP card passes
/// through), and auto-removed at the start of the owner's turn so it re-arms each turn.
/// Cards that refund NP on their own play (e.g. Siegfried's Balmung-EX) capture whether it
/// was already set BEFORE consuming, then refund full on the first ult of the turn and the
/// reduced amount on any later one (<see cref="NpCharge.RefundAfterNpCard"/>) — a legitimate
/// two-ult turn stays possible but costs a net ≥50 instead of free-doubling.
/// </summary>
public sealed class NpResolvedThisTurnPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) await PowerCmd.Remove(this);
    }
}
