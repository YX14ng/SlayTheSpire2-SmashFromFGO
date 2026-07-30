using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// Base local de modularización (AUDIT-2026-06-15): RoundTableFragment y LordCamelotRestored
/// son el MISMO motor "Baluarte" (retención de Bloqueo entre turnos + golpe totalmente
/// bloqueado → Estrellas + perder Vida → Carga NP, con cap anti multi-hit de procs/turno),
/// sólo cambian los montos, la rareza y si una releva a la otra. Esta base concentra el motor
/// y el cap; las subclases sólo declaran su curva (constantes), su <see cref="Rarity"/> y, si
/// corresponde, <see cref="IsActive"/>. Comportamiento idéntico al de las dos relics previas.
/// </summary>
public abstract class BulwarkEngineRelic : MashShielderRelic, IBlockRetentionSource
{
    /// <summary>Bloqueo máximo conservado entre turnos.</summary>
    protected abstract int MaxRetainedBlock { get; }

    /// <summary>Estrellas de Crítico por golpe enemigo totalmente bloqueado.</summary>
    protected abstract int StarsPerFullBlock { get; }

    /// <summary>Carga NP por perder Vida.</summary>
    protected abstract int NpPerHpLoss { get; }

    /// <summary>Tope de procs por turno (por conversión, candado anti multi-hit — parche P1).</summary>
    protected virtual int MaxProcsPerTurn => 3;

    /// <summary>
    /// Si esta relic está vigente. Por defecto siempre (LordCamelotRestored); una subclase que
    /// puede ser relevada por otra (RoundTableFragment) lo sobrescribe. Cuando es false, ni
    /// retiene Bloqueo ni convierte.
    /// </summary>
    protected virtual bool IsActive => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(MaxRetainedBlock, ValueProp.Unpowered),
        new DynamicVar("Stars", StarsPerFullBlock),
        new DynamicVar("NpCharge", NpPerHpLoss),
        new DynamicVar("MaxProcs", MaxProcsPerTurn)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CritStarsPower>()
    ];

    public decimal RetentionCap(Creature creature) => IsActive ? MaxRetainedBlock : 0m;

    public override async Task BeforeCombatStartLate()
    {
        await Powers.Forms.Forms.Enter<Powers.Forms.ShielderFormPower>(null, Owner.Creature, null);
    }

    /// <summary>Camino ÚNICO del pago de estrellas por golpe totalmente bloqueado, con el candado
    /// P1 (máx 3 procs/turno). Lo usan el listener normal (abajo) y la Pared Absoluta (P8b) — antes
    /// la Pared duplicaba el pago a mano SIN el candado (audit 2026-07-04). Devuelve true si pagó.</summary>
    public Task<bool> TryProcFullBlockStars() =>
        TryProcFullBlockStars(new BlockingPlayerChoiceContext());

    public async Task<bool> TryProcFullBlockStars(PlayerChoiceContext choiceContext)
    {
        if (!IsActive || FgoCombatState.GetTurn(Owner.Creature, 6, 2) >= MaxProcsPerTurn) return false;
        await FgoCombatState.IncrementTurn(
            choiceContext, Owner.Creature, 6, MaxProcsPerTurn, null, width: 2);
        Flash();
        await CritStars.Gain(choiceContext, Owner.Creature, StarsPerFullBlock, null);
        return true;
    }

    /// <summary>Golpe enemigo totalmente bloqueado → estrellas (máx 3/turno, parche P1).</summary>
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!IsActive || target != Owner.Creature || dealer == null) return;
        if (!props.IsPoweredAttack() || !result.WasFullyBlocked) return;
        await TryProcFullBlockStars(choiceContext);
    }

    /// <summary>Perder Vida → Carga NP (máx 3/turno, parche P1).</summary>
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!IsActive || creature != Owner.Creature || delta >= 0) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 8, 2) >= MaxProcsPerTurn) return;
        var context = new BlockingPlayerChoiceContext();
        await FgoCombatState.IncrementTurn(
            context, Owner.Creature, 8, MaxProcsPerTurn, null, width: 2);
        Flash();
        await NpCharge.Gain(context, Owner.Creature, NpPerHpLoss, null);
    }

    public override bool ShouldClearBlock(Creature creature) => creature != Owner.Creature;

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != Owner.Creature) return;

        if (creature.Block == 0) return;
        await BlockRetention.Enforce(creature);
        Flash();
    }
}
