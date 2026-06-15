using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Curses;

/// <summary>
/// Maldición (诅咒) — FGO curse as a debuff: at the start of the cursed creature's
/// turn it takes damage equal to its Curse (ignores Block), then the Curse decays
/// by 1. Capped at <see cref="MaxPerEnemy"/> per enemy (enforced by <see cref="Curses"/>). Mirrors the
/// vanilla PoisonPower pattern exactly (timing, decrement, throwing context).
/// Personal offense: does NOT scale in multiplayer.
/// </summary>
public sealed class CursePower : FGOCorePower
{
    // 15 -> 25 en el re-balance v3: con los hexes de HextechRunes las peleas son mas
    // largas y el tope de 15 ahogaba el arquetipo de Maldicion justo cuando escalaba.
    public const int MaxPerEnemy = 25;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead) return;

        Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        if (Owner.IsAlive && !DecayIsStopped())
        {
            await PowerCmd.Decrement(this);
        }
    }

    private bool DecayIsStopped()
    {
        foreach (var opponent in Owner.CombatState.GetOpponentsOf(Owner))
        {
            if (opponent.IsDead) continue;
            foreach (var power in opponent.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>())
            {
                if (power is ICursePreserver) return true;
            }
        }
        return false;
    }
}
