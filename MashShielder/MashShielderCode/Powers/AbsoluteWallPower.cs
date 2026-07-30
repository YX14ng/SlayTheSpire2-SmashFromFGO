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

    /// <summary>Committed HP loss prevented by the wall in the current damage event.</summary>
    private bool _preventedHit;

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return amount;
        return 0m;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        _preventedHit = true;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || !_preventedHit) return;
        _preventedHit = false;

        // Guard P8b: con Bloqueo parcial de por medio el motor ya computa
        // WasFullyBlocked=true (el resto lo anulamos en el hook de HP) y los listeners
        // normales (InterceptPower, SenpaiPromisePower, reliquia) disparan solos.
        if (result.WasFullyBlocked || dealer == null || dealer == Owner || !props.IsPoweredAttack()) return;

        Flash();

        // 1) Intercepción (permanente + de turno: ProvokePower deriva de InterceptPower).
        //    GetPowerInstances<InterceptPower> directo (audit: el doble filtro OfType alocaba de más
        //    en un hook que corre por cada golpe enemigo detenido).
        var intercept = Owner.GetPowerInstances<InterceptPower>().Sum(p => p.Amount);
        if (intercept > 0 && !dealer.IsDead)
        {
            await CreatureCmd.Damage(choiceContext, dealer, intercept, ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner);
        }

        // 2) Estrellas de la reliquia inicial — por el MISMO camino y candado (3 procs/turno, P1)
        //    que el golpe totalmente bloqueado normal (audit 2026-07-04: acá se pagaba a mano, sin
        //    cupo y con montos duplicados del valor de la reliquia).
        var engine = Owner.Player?.Relics.OfType<BulwarkEngineRelic>().FirstOrDefault();
        if (engine != null)
        {
            await engine.TryProcFullBlockStars(choiceContext);
        }

        // 3) Promesa a Senpai: +NP por golpe detenido.
        var promise = Owner.GetPowerAmount<SenpaiPromisePower>();
        if (promise > 0)
        {
            await NpCharge.Gain(choiceContext, Owner, promise, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}
