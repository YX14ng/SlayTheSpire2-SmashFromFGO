using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// El Peso de las Expectativas (期望之重) — DESIGN-SIEGFRIED §7. Al fin de tu turno, si NO jugaste
/// Ataques: +<see cref="NpPerTrigger"/> NP y +1 Sangre de Dragón, con tope de 2 activaciones/turno (P3).
/// Detección por flag PROPIO (CombatState no expone "ataques jugados este turno"): el gate de tipo
/// (EnragePower) + gate de dueño (AfterimagePower) marca _playedAttackThisTurn. La ulti Balmung ES
/// Ataque → marca el flag → el Power NO proca en turnos de ult: el auto-límite §7 sale gratis de la
/// semántica de tipo. Amount = SOLO contador de stacks (Counter); NpPerTrigger es campo settable que la
/// carta fija desde su DynamicVar (para que el up 20→30 se refleje sin chocar con el conteo de stacks).
/// </summary>
public sealed class WeightOfExpectationsPower : SiegfriedPower
{
    public const int MaxTriggersPerTurn = 2;
    public const int ScalesPerTrigger = 1;

    public int NpPerTrigger = 20;

    private bool _playedAttackThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == Owner)
        {
            _playedAttackThisTurn = true;
        }
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _playedAttackThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side && !_playedAttackThisTurn)
        {
            for (var i = 0; i < System.Math.Min(Amount, MaxTriggersPerTurn); i++)
            {
                await NpCharge.Gain(Owner, NpPerTrigger, null);
                await PowerCmd.Apply<DragonScalesPower>(Owner, ScalesPerTrigger, Owner, null);
            }
            Flash();
        }
        _playedAttackThisTurn = false;
    }
}
