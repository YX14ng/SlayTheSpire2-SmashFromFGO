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
/// Lord Camelot (restaurado) — Ancient relic, rediseño v2: retiene hasta 25 de
/// Bloqueo entre turnos y SUBE LOS MONTOS del motor de la starter (arco Hellup de
/// Jeanne): golpe enemigo totalmente bloqueado → +20 Estrellas de Crítico; perder
/// Vida → +20 de Carga NP. Parche P1 del juez: mantiene el cap de 3 procs por turno
/// y por conversión (techo 60/turno) — sube el monto, NO duplica los procs.
/// Supersedes the Round Table Fragment (caps y conversiones no se apilan; esta gana).
/// </summary>
public sealed class LordCamelotRestored : MashShielderRelic, IBlockRetentionSource
{
    public const int MaxRetainedBlock = 25;
    public const int StarsPerFullBlock = 20;
    public const int NpPerHpLoss = 20;
    public const int MaxProcsPerTurn = 3;

    private int _starProcsThisTurn;
    private int _npProcsThisTurn;

    public override RelicRarity Rarity => RelicRarity.Ancient;

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

    public decimal RetentionCap(Creature creature) => MaxRetainedBlock;

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
        if (target != Owner.Creature || dealer == null) return;
        if (!props.IsPoweredAttack() || !result.WasFullyBlocked) return;
        if (_starProcsThisTurn >= MaxProcsPerTurn) return;
        _starProcsThisTurn++;
        Flash();
        await CritStars.Gain(Owner.Creature, StarsPerFullBlock, null);
    }

    /// <summary>Perder Vida → Carga NP (máx 3/turno, parche P1).</summary>
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner.Creature || delta >= 0) return;
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
