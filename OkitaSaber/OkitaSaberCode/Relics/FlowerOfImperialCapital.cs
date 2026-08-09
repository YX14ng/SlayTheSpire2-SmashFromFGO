using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
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
/// Flor de la Capital Imperial (帝都之华) — reliquia ANCIENT/JEFE (DESIGN-OKITA §6.2) que reemplaza al
/// Haori Asagi: DUPLICA ambas conversiones del motor de Okita.
///   (1) cada vez que jugás un Ataque: +<see cref="StarsPerAttack"/> *Estrellas (máx.
///       <see cref="HaoriAsagi.MaxProcsPerTurn"/> procs/turno, reset al inicio de tu turno);
///   (2) cada vez que uno de tus ataques CRITICA (consume *Crítico Listo): +<see cref="NpPerCrit"/> NP.
///
/// Misma maquinaria que HaoriAsagi (AfterCardPlayed para el ★-por-Ataque; AfterPowerAmountChanged con
/// amount < 0 sobre CritReadyPower para el NP-por-crítico), con los números al doble (20★ / 40 NP).
/// Como Orobas elimina el Haori, la Flor también instala el Aliento inicial, su regeneración y el
/// contador de ataques; no depende de que ambas reliquias coexistan.
/// </summary>
public sealed class FlowerOfImperialCapital : OkitaRelic, ICriticalConsumedListener
{
    public const int StarsPerAttack = HaoriAsagi.StarsPerAttack * 2; // 20
    public const int NpPerCrit = HaoriAsagi.NpPerCrit * 2;           // 40

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<CritReadyPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Aliento.ResetHitZero(Owner.Creature);
        await AttacksThisTurnPower.EnsureInstalled(Owner.Creature);
        await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<AlientoPower>(
            new BlockingPlayerChoiceContext(), Owner.Creature,
            AlientoPower.StartingBreath, Owner.Creature, null);
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return;
        Aliento.ResetHitZero(Owner.Creature);

        var regen = AlientoPower.RegenPerTurn;
        foreach (var booster in FGOCore.FGOCoreCode.Listeners.PowersOf<IBreathRegenBooster>(Owner.Creature))
            regen += booster.ExtraBreathRegen;

        await Aliento.Gain(choiceContext, Owner.Creature, regen, null);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner.Creature) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 5, 2) >= HaoriAsagi.MaxProcsPerTurn) return;
        await FgoCombatState.IncrementTurn(
            context, Owner.Creature, 5, HaoriAsagi.MaxProcsPerTurn, cardPlay.Card, width: 2);
        Flash();
        await CritStars.Gain(context, Owner.Creature, StarsPerAttack, null);
    }

    // amount < 0 sobre CritReadyPower = un Crítico Listo CONSUMIDO (un crítico consumado) → +NP.
    public async Task AfterCriticalConsumed(PlayerChoiceContext choiceContext, CriticalHit critical)
    {
        if (critical.Owner != Owner.Creature) return;
        Flash();
        await NpCharge.Gain(choiceContext, Owner.Creature, NpPerCrit, null);
        if (FgoCombatState.GetTurn(Owner.Creature, 7) == 0)
        {
            await FgoCombatState.SetTurn(
                choiceContext, Owner.Creature, 7, 1, critical.Card);
            await Aliento.Gain(choiceContext, Owner.Creature, HaoriAsagi.BreathPerCrit, null);
        }
    }
}
