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
public sealed class HaoriAsagi : OkitaRelic
{
    public const int StarsPerAttack = 10;
    public const int MaxProcsPerTurn = 3;
    public const int NpPerCrit = 20;

    public override RelicRarity Rarity => RelicRarity.Starter;

    private int _attackProcsThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<AlientoPower>()
    ];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        _attackProcsThisTurn = 0;
        // Aliento inicial del combate (6) — el embudo arranca con margen para una Ráfaga.
        await PowerCmd.Apply<AlientoPower>(Owner.Creature, AlientoPower.StartingBreath, Owner.Creature, null);
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _attackProcsThisTurn = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner.Creature) return;
        if (_attackProcsThisTurn >= MaxProcsPerTurn) return;
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
