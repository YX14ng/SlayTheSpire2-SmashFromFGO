using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Forms;

/// <summary>
/// Base for a character's form/stance powers. Only one is active at a time; switching
/// goes through <see cref="FormSwitch"/>. Provides shared infrastructure: fully-blocked
/// hit tracking (for counter mechanics) and the visual frames hook for
/// <see cref="FormVisuals"/>. Character passives live in subclasses.
/// </summary>
public abstract class FormPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>res:// path of this form's SpriteFrames, or null to keep the current sprite.</summary>
    public virtual string? FramesPath => null;

    /// <summary>A permanent form can never be replaced once entered.</summary>
    public virtual bool IsPermanent => false;

    /// <summary>Enemy hits on Owner fully stopped by Block this turn (for counter mechanics).</summary>
    public int BlockedHitsThisTurn { get; private set; }

    public static int GetBlockedHits(Creature creature) =>
        creature.GetPowerInstances<FormPower>().FirstOrDefault()?.BlockedHitsThisTurn ?? 0;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            BlockedHitsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && dealer != null && props.IsPoweredAttack() && result.WasFullyBlocked)
        {
            BlockedHitsThisTurn++;
        }
        return Task.CompletedTask;
    }
}
