using System.Collections.Generic;
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

    /// <summary>Carry the per-turn blocked-hit count onto a freshly-applied form so it survives a
    /// mid-turn form switch (the count is per-turn, not per-form-instance). Called by
    /// <see cref="FormSwitch.Enter"/> right after the new form is applied.</summary>
    public void SeedBlockedHits(int hits) => BlockedHitsThisTurn = hits;

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // Reset al arrancar el turno ENEMIGO, no el del jugador (audit 2026-07-04): los golpes se
        // bloquean durante la volea enemiga N y las cartas de contraataque (Reprisal, CounterBlade)
        // los leen en el turno del jugador N+1. Con el reset en el turno del jugador, esas cartas
        // leían siempre 0. Ahora el conteo persiste todo el turno del jugador y se limpia recién
        // cuando empieza la siguiente volea enemiga.
        if (side == CombatSide.Enemy)
        {
            BlockedHitsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    // Retención de Bloqueo (audit 2026-07-04): implementar IBlockRetentionSource en una forma NO
    // prevenía el clear por sí solo — el juego solo pregunta ShouldClearBlock, y sin un BulwarkPower
    // presente nadie respondía false, así que la retención de la forma estaba muerta. La base FormPower
    // responde por cualquier subclase que implemente la interfaz con cap > 0; Enforce unifica el
    // resultado con el resto de las fuentes (el juego elige UN solo preventer).
    public override bool ShouldClearBlock(Creature creature)
    {
        if (creature == Owner && this is Block.IBlockRetentionSource src && src.RetentionCap(creature) > 0m)
        {
            return false;
        }
        return base.ShouldClearBlock(creature);
    }

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != Owner) return;
        await Block.BlockRetention.Enforce(creature);
        Flash();
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
