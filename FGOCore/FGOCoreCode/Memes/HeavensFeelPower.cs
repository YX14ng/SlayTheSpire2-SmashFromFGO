using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Heaven's Feel (天之杯) — tu PRÓXIMO Ataque con poder hace ×1.4 (+40%), sin coste de HP
/// (a diferencia del Grial Negro). Se consume tras el primer Ataque jugado y al final del
/// turno (patrón <c>NextAttackBoostPower</c> de Artoria). Como las cartas-NP del ecosistema
/// resuelven su daño vía <c>DamageCmd.Attack</c> (Ataque con poder), este buff cubre tanto
/// el "próximo NP" como el "próximo ataque". Single: no apila el multiplicador.
/// Personal: no escala en multijugador.
/// </summary>
public sealed class HeavensFeelPower : FGOCorePower
{
    public const decimal DamageMultiplier = 1.4m;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageMultiplicativeFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 1m;
        return DamageMultiplier;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
