using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// Fragmento de la Mesa Redonda — starter relic, rediseño v2: de retención pasiva a
/// MOTOR (estilo la reliquia de Jeanne). (1) Al final de tu turno conserva hasta 10
/// de Bloqueo y entra en forma Shielder. (2) Golpe enemigo totalmente bloqueado →
/// +10 Estrellas de Crítico (tanquear ES generar; mismo trigger que Intercepción/
/// SenpaiPromise). (3) Perder Vida → +10 de Carga NP (costo = recurso). Parche P1
/// del juez: cada conversión proca máximo 3 veces por turno (reset al inicio del
/// turno del jugador) — candado anti multi-hit. Si el dueño tiene LordCamelotRestored,
/// esa reliquia toma el relevo (+20 por proc, mismos caps) y esta no convierte.
/// </summary>
public sealed class RoundTableFragment : MashShielderRelic, IBlockRetentionSource
{
    public const int MaxRetainedBlock = 10;
    public const int StarsPerFullBlock = 10;
    public const int NpPerHpLoss = 10;
    public const int MaxProcsPerTurn = 3;

    private int _starProcsThisTurn;
    private int _npProcsThisTurn;

    public override RelicRarity Rarity => RelicRarity.Starter;

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

    /// <summary>LordCamelotRestored supersedes both the retention cap and the conversions.</summary>
    private bool IsSuperseded => Owner.GetRelic<LordCamelotRestored>() != null;

    public decimal RetentionCap(Creature creature) => IsSuperseded ? 0m : MaxRetainedBlock;

    public override async Task BeforeCombatStartLate()
    {
        _starProcsThisTurn = 0;
        _npProcsThisTurn = 0;
        await Powers.Forms.Forms.Enter<Powers.Forms.ShielderFormPower>(null, Owner.Creature, null);
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _starProcsThisTurn = 0;
            _npProcsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    /// <summary>Golpe enemigo totalmente bloqueado → estrellas (máx 3/turno, parche P1).</summary>
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (IsSuperseded || target != Owner.Creature || dealer == null) return;
        if (!props.IsPoweredAttack() || !result.WasFullyBlocked) return;
        if (_starProcsThisTurn >= MaxProcsPerTurn) return;
        _starProcsThisTurn++;
        Flash();
        await CritStars.Gain(Owner.Creature, StarsPerFullBlock, null);
    }

    /// <summary>Perder Vida → Carga NP (máx 3/turno, parche P1).</summary>
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (IsSuperseded || creature != Owner.Creature || delta >= 0) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (_npProcsThisTurn >= MaxProcsPerTurn) return;
        _npProcsThisTurn++;
        Flash();
        await NpCharge.Gain(Owner.Creature, NpPerHpLoss, null);
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
