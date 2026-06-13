using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// La Reina (Berserker, 妖精女王) — forma inicial de Morgan. EL DETONADOR del motor de
/// dos tiempos (rediseño 2026-06-12): es donde COSECHÁS la maldición sembrada en Caster.
/// (a) "Sentencia": cuando un Ataque tuyo daña HP de un enemigo maldito, consume TODA
///     su Maldición y le inflige daño adicional igual a lo consumido (golpe aparte).
///     Genera poca maldición propia → te empuja a sembrar en Caster y volver a cosechar.
/// (b) La primera vez que dañás HP enemigo cada turno: +10 NP.
/// Ya NO amplifica Maldición (ICurseAmplifier se movió a la Bruja/Caster, que siembra).
/// </summary>
public sealed class FairyQueenFormPower : MorganFormPower
{
    public const int NpOnDamage = 10;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_queen.tres";

    private bool _npThisTurn;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (side == CombatSide.Player)
        {
            _npThisTurn = false;
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (dealer != Owner || target.IsPlayer || target.IsDead) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        // (b) Primera carga NP por daño del turno.
        if (!_npThisTurn)
        {
            _npThisTurn = true;
            Flash();
            await NpCharge.Gain(Owner, NpOnDamage, null);
        }

        // (a) Sentencia: detoná la Maldición del objetivo. El golpe extra es Unpowered
        // (no re-dispara este hook ni la carga por daño; con la Maldición ya en 0 sería
        // no-op de todos modos).
        var curse = Curses.Of(target);
        if (curse <= 0) return;
        var consumed = await Curses.Consume(target, curse);
        if (consumed <= 0) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, target, consumed,
            ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner, null);
    }
}
