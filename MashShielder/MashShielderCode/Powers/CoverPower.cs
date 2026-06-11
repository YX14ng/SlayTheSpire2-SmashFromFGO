using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Cobertura — until Mash's next turn, any damage that pierces an allied player's
/// defenses is taken by Mash instead (her own Block applies). Multiplayer mechanic:
/// the ally's HP loss is negated at the HP-loss stage and re-dealt to Mash right after.
/// </summary>
public sealed class CoverPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    private decimal _pendingTransfer;

    private bool Covers(Creature target, Creature? dealer) =>
        target != Owner && target.IsPlayer && !target.IsDead &&
        dealer != null && dealer.IsMonster && !Owner.IsDead;

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!Covers(target, dealer) || amount <= 0) return amount;
        _pendingTransfer += amount;
        return 0m;
    }

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_pendingTransfer <= 0 || !Covers(target, dealer)) return;

        var dmg = _pendingTransfer;
        _pendingTransfer = 0;
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, dmg, ValueProp.Move, dealer, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
