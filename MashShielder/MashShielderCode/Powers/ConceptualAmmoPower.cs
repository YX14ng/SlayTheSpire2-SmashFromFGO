using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>Munición Conceptual — once per turn, your Attacks strip one buff from the enemy hit.</summary>
public sealed class ConceptualAmmoPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private bool _usedThisTurn;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _usedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (_usedThisTurn || dealer != Owner || cardSource == null || !props.IsPoweredAttack()) return;
        if (target.IsDead) return;
        if (!target.GetPowerInstances<PowerModel>().Any(p => p.Type == PowerType.Buff)) return;

        _usedThisTurn = true;
        Flash();
        await BlackBarrel.RemoveBuffs(target, 1);
    }
}
