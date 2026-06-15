using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Saturación de Eventos (事象饱和) — ESTE TURNO tus Ataques IGNORAN Bloqueo (DESIGN-OKITA §5.4:
/// la paradoja como skill, anti torre-de-bloqueo). La aplica «Saturación de Eventos» (2⚡, Hab,
/// Exhaust). Se auto-remueve al terminar tu turno (patrón GloryEdgePower / ExposedBackPower).
///
/// "Ignorar Bloqueo" de verdad (no despojar): justo ANTES de que el daño de un Ataque de carta del
/// owner se aplique a SU objetivo (hook <see cref="BeforeDamageReceived"/>, que el motor invoca
/// antes de DamageBlockInternal — CreatureCmd.cs:143/145), vacía el Bloqueo de ESE objetivo concreto.
/// Así el golpe pasa entero pero no se toca el Bloqueo de enemigos no golpeados (patrón ThornsPower).
/// Single, personal: no escala en multijugador.
/// </summary>
public sealed class EventSaturationPower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.BeforeDamageReceived(choiceContext, target, amount, props, dealer, cardSource);
        // Solo Ataques de carta del owner contra un enemigo con Bloqueo: vaciá el de ESE objetivo.
        if (dealer != Owner || cardSource == null || !props.IsPoweredAttack()) return;
        if (target.Side == Owner.Side || target.IsDead || target.Block <= 0) return;
        Flash();
        await CreatureCmd.LoseBlock(target, target.Block);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}
