using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

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

    public int NpPerTrigger => Math.Max(20, FgoCombatState.GetCombat(Owner, 0, 6));

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public Task Configure(PlayerChoiceContext context, int npPerTrigger, CardModel source) =>
        FgoCombatState.SetCombat(
            context, Owner, 0, Math.Max(NpPerTrigger, npPerTrigger), source, width: 6);

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == Owner)
        {
            await FgoCombatState.SetTurn(context, Owner, 0, 1, cardPlay.Card);
        }
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner) && FgoCombatState.GetTurn(Owner, 0) == 0)
        {
            for (var i = 0; i < System.Math.Min(Amount, MaxTriggersPerTurn); i++)
            {
                await NpCharge.Gain(choiceContext, Owner, NpPerTrigger, null);
                await PowerCmd.Apply<DragonScalesPower>(choiceContext, Owner, ScalesPerTrigger, Owner, null);
            }
            Flash();
        }
    }
}
