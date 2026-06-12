using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Power interno de «Espada de Selección EX» — tu próximo Ataque este turno hace
/// este daño adicional. Se quita tras el primer Ataque jugado y al final del turno
/// (patrón Muro Móvil de Mash).
/// </summary>
public sealed class NextAttackBoostPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    // Counter: el monto del boost vive en Amount (con Single el apply podría
    // normalizarlo a 1; mismo patrón que MobileWallPower de Mash).
    public override PowerStackType StackType => PowerStackType.Counter;

    // ModifyDamageAdditive es DELTA (default 0).
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return Amount;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
