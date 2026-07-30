using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.CardTypes;

/// <summary>
/// Fuerza TEMPORAL (este turno) que otorga el bonus Buster normal de <see cref="CommandBonusPower"/>:
/// +<c>Amount</c> de daño a tus Ataques con poder este turno, auto-removido al final
/// de tu turno. Es un power propio de FGOCore (no la vanilla <c>TemporaryStrengthPower</c>, que exige un
/// <c>OriginModel</c> = carta/reliquia concreta — FGOCore no tiene una carta única dueña del bonus).
/// El efecto +daño aditivo es idéntico a <c>StrengthPower.ModifyDamageAdditive</c> (mismo gate
/// <c>props.IsPoweredAttack()</c>); patrón <c>NextAttackBoostPower</c> de Artoria
/// + auto-remoción al fin de turno (patrón <c>GloryEdgePower</c>). Counter: aplicaciones del mismo
/// turno suman. La Fuerza PERMANENTE del bonus Buster-ulti usa la vanilla <c>StrengthPower</c> directo.
/// </summary>
public sealed class CommandBusterStrengthPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // VISIBLE: el jugador ve "+N daño este turno". Sin png dedicado, el icono base de FGOCorePower
    // ({id}.png) cae al fallback power.png vía PowerImagePath. PENDIENTE DE ARTE: commandbusterstrengthpower.png
    // (+ big/) en FGOCore/images/powers/ para un icono propio; el fallback no rompe el build.

    // Personal: el +daño aplica solo a los Ataques del dueño; no escala en multijugador.
    public override bool ShouldScaleInMultiplayer => false;

    // ModifyDamageAdditive es DELTA (default 0): mismo gate que StrengthPower (solo Ataques con poder
    // del propio dueño). No toca el StrengthPower vanilla, así que no contamina contadores de Fuerza.
    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return Amount;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
