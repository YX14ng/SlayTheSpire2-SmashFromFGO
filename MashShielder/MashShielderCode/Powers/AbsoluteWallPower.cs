using MashShielder.MashShielderCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Pared Absoluta — until your next turn, your HP cannot be reduced.
/// Parche P8b del rediseño v2: los golpes detenidos por este efecto cuentan como
/// totalmente bloqueados. En vez de sintetizar result.WasFullyBlocked, el power invoca
/// los tres efectos directamente al prevenir pérdida de Vida de un Ataque enemigo:
/// daño de Intercepción al atacante, Estrellas de Crítico de la reliquia (+10/+20) y
/// Carga NP de SenpaiPromise. Guard anti doble-disparo: si el golpe YA quedó totalmente
/// bloqueado de verdad (el motor marca WasFullyBlocked cuando el resto se anula aquí y
/// había Bloqueo de por medio), los listeners normales ya dispararon y este power calla.
/// </summary>
public sealed class AbsoluteWallPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>HP loss from an enemy attack hit prevented by the wall (per-hit flag).</summary>
    private decimal _preventedHit;

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return amount;
        if (amount > 0 && dealer != null && dealer != Owner && props.IsPoweredAttack())
        {
            _preventedHit = amount;
        }
        return 0m;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || _preventedHit <= 0) return;
        _preventedHit = 0;

        // Guard P8b: con Bloqueo parcial de por medio el motor ya computa
        // WasFullyBlocked=true (el resto lo anulamos en el hook de HP) y los listeners
        // normales (InterceptPower, SenpaiPromisePower, reliquia) disparan solos.
        if (result.WasFullyBlocked || dealer == null || !props.IsPoweredAttack()) return;

        Flash();

        // 1) Intercepción (permanente + de turno: ProvokePower deriva de InterceptPower).
        var intercept = Owner.GetPowerInstances<PowerModel>().OfType<InterceptPower>().Sum(p => p.Amount);
        if (intercept > 0 && !dealer.IsDead)
        {
            await CreatureCmd.Damage(choiceContext, dealer, intercept, ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner, null);
        }

        // 2) Estrellas de Crítico de la reliquia inicial (golpe totalmente bloqueado →
        //    +10; LordCamelotRestored: +20). Montos del parche P1 del rediseño.
        var player = Owner.Player;
        var stars = player?.GetRelic<LordCamelotRestored>() != null ? 20
            : player?.GetRelic<RoundTableFragment>() != null ? 10 : 0;
        if (stars > 0)
        {
            await FGOCore.FGOCoreCode.Stars.CritStars.Gain(Owner, stars, null);
        }

        // 3) Promesa a Senpai: +NP por golpe detenido.
        var promise = Owner.GetPowerAmount<SenpaiPromisePower>();
        if (promise > 0)
        {
            await NpCharge.Gain(Owner, promise, null);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
