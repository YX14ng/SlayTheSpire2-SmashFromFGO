using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Haori Asagi del Shinsengumi (新选组浅葱羽织) — la reliquia STARTER y el MOTOR de Okita
/// (DESIGN-OKITA §6.1). Convierte sus dos eventos universales en sus dos economías:
/// (1) cada vez que jugás un Ataque: +<see cref="StarsPerAttack"/> *Estrellas (máx. <see cref="MaxProcsPerTurn"/>
///     procs/turno, reset al inicio de tu turno);
/// (2) cada vez que uno de tus ataques CRITICA (consume *Crítico Listo): +<see cref="NpPerCrit"/> Carga NP.
///
/// También fija el *Aliento inicial del combate (<see cref="AlientoPower.StartingBreath"/> = 6): el
/// contador del embudo vive con el motor, no en la clase del personaje (patrón LindenLeaf/SeaOfLifeWomb).
///
/// Detección de crítico SIN API nueva de FGOCore: CritReadyPower se decrementa al consumirse
/// (AfterCardPlayed → Decrement). Lo leemos en <see cref="AfterPowerAmountChanged"/> con amount < 0
/// = un crítico consumado. (El Haori es la única clienta hoy; si FGOCore agrega CritReady.Consumed,
/// este hook se reemplaza por el callback — ver doccomment §4 del diseño.)
/// </summary>
public sealed class HaoriAsagi : OkitaRelic, ICriticalConsumedListener
{
    public const int StarsPerAttack = 10;
    public const int MaxProcsPerTurn = 3;
    public const int NpPerCrit = 20;
    public const int BreathPerCrit = 1; // reembolso de Aliento al criticar (cap 1/turno, P1)

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<FlowerOfImperialCapital>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<AlientoPower>()
    ];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Aliento.ResetHitZero(Owner.Creature);
        // Contador de Ataques del turno instalado DESDE el arranque (audit 2026-07-04): instalado
        // perezosamente por las cartas, los Ataques previos del turno no se contaban (mismo fix que
        // OathOfUruk con CardsThisTurnPower).
        await AttacksThisTurnPower.EnsureInstalled(Owner.Creature);
        // Aliento inicial del combate (6) — el embudo arranca con margen para una Rafaga.
        await PowerCmd.Apply<AlientoPower>(new BlockingPlayerChoiceContext(), Owner.Creature, AlientoPower.StartingBreath, Owner.Creature, null);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return;
        Aliento.ResetHitZero(Owner.Creature);

        // REGEN del Aliento (audit 2026-07-04): vivia en AlientoPower.AfterSideTurnStart, pero ese
        // power se REMUEVE al llegar a 0 — y con el moria el regen para el resto del combate. La
        // starter (siempre presente) regenera; Aliento.Gain reinstala el power si falta y respeta Cap.
        var regen = AlientoPower.RegenPerTurn;
        foreach (var b in FGOCore.FGOCoreCode.Listeners.PowersOf<IBreathRegenBooster>(Owner.Creature))
        {
            regen += b.ExtraBreathRegen;
        }
        await Aliento.Gain(choiceContext, Owner.Creature, regen, null);
    }

    /// <summary>La Flor de la Capital Imperial REEMPLAZA este motor (numeros al doble): mientras
    /// este presente, los dos procs del Haori callan (audit 2026-07-04: antes se apilaban y el motor
    /// se triplicaba). El regen/reset de Aliento del Haori NO se apaga — la Flor no lo cubre.</summary>
    private bool ReplacedByFlower()
    {
        var relics = Owner.Creature.Player?.Relics;
        if (relics == null) return false;
        foreach (var r in relics)
        {
            if (r is FlowerOfImperialCapital) return true;
        }
        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (ReplacedByFlower()) return;
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner.Creature) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 5, 2) >= MaxProcsPerTurn) return;
        await FgoCombatState.IncrementTurn(
            context, Owner.Creature, 5, MaxProcsPerTurn, cardPlay.Card, width: 2);
        Flash();
        await CritStars.Gain(context, Owner.Creature, StarsPerAttack, null);
    }

    // amount < 0 sobre CritReadyPower = un Crítico Listo CONSUMIDO (un crítico consumado) → +NP.
    // FIX HOMOGENEIZACIÓN (P1 Okita): además REEMBOLSA 1 *Aliento (cap 1/turno) — la velocidad se
    // retroalimenta (el iai perfecto no te deja sin aire). Hace que el Crítico de Okita exprese su
    // recurso de firma, no solo el ×2 genérico. El cap evita el loop ⚡-positivo (perilla #4 del doc).
    public async Task AfterCriticalConsumed(PlayerChoiceContext choiceContext, CriticalHit critical)
    {
        if (ReplacedByFlower()) return;
        if (critical.Owner != Owner.Creature) return;
        Flash();
        await NpCharge.Gain(choiceContext, Owner.Creature, NpPerCrit, null);
        if (FgoCombatState.GetTurn(Owner.Creature, 7) == 0)
        {
            await FgoCombatState.SetTurn(
                choiceContext, Owner.Creature, 7, 1, critical.Card);
            await Aliento.Gain(choiceContext, Owner.Creature, BreathPerCrit, null);
        }
    }
}
