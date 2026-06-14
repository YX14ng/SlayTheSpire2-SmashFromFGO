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
/// NO fija el Aliento inicial: el Haori al que reemplaza ya lo hace, y un Ancient asume que el motor base
/// ya está en juego (la Capital Imperial es el "te lo cambio por una versión más poderosa").
/// </summary>
public sealed class FlowerOfImperialCapital : OkitaRelic
{
    public const int StarsPerAttack = HaoriAsagi.StarsPerAttack * 2; // 20
    public const int NpPerCrit = HaoriAsagi.NpPerCrit * 2;           // 40

    public override RelicRarity Rarity => RelicRarity.Ancient;

    private int _attackProcsThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<CritReadyPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    public override Task BeforeCombatStartLate()
    {
        _attackProcsThisTurn = 0;
        return base.BeforeCombatStartLate();
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _attackProcsThisTurn = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner.Creature) return;
        if (_attackProcsThisTurn >= HaoriAsagi.MaxProcsPerTurn) return;
        _attackProcsThisTurn++;
        Flash();
        await CritStars.Gain(Owner.Creature, StarsPerAttack, null);
    }

    // amount < 0 sobre CritReadyPower = un Crítico Listo CONSUMIDO (un crítico consumado) → +NP.
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount >= 0m || power is not CritReadyPower || power.Owner != Owner.Creature) return;
        Flash();
        await NpCharge.Gain(Owner.Creature, NpPerCrit, null);
    }
}
