using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>Servomotores del Ortinax — end your turn having played 2+ Attacks: gain this much
/// Block AND +10 NP Charge (rediseño v2: los servos reponen el Bloqueo gastado atacando
/// y cargan el medidor — el plan del Ortinax por fin paga).</summary>
public sealed class OrtinaxServosPower : MashShielderPower
{
    /// <summary>NP fijo por proc (denominación 10; no escala con el upgrade).</summary>
    public const int NpPerProc = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner)
        {
            await FgoCombatState.IncrementTurn(context, Owner, 1, 2, cardPlay.Card, width: 2);
        }
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || FgoCombatState.GetTurn(Owner, 1, 2) < 2) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null);
        await NpCharge.Gain(choiceContext, Owner, NpPerProc, null);
    }
}
