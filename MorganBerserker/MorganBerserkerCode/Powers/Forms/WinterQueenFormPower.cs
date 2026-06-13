using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Reina del Invierno (冬之女王) — forma clímax PERMANENTE (solo por carta rara): la
/// válvula "ambas a la vez" del motor de dos tiempos. SIEMBRA Y DETONA sin penalidad:
/// (a) Amplifica Maldición +2 (ICurseAmplifier) y NO decae (ICursePreserver) — como Caster.
/// (b) "Sentencia": tus Ataques consumen la Maldición del objetivo por daño extra — como Berserker.
/// (c) Al inicio de tu turno: +8 NP. (d) Primera vez que dañás HP enemigo cada turno: +10 NP.
/// Es la meta aspiracional del mazo (no el modo cómodo inicial).
/// </summary>
public sealed class WinterQueenFormPower : MorganFormPower, ICurseAmplifier, ICursePreserver
{
    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_winter.tres";

    public override bool IsPermanent => true;

    public int ExtraCurse => 2;

    private bool _npThisTurn;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (side == CombatSide.Player)
        {
            _npThisTurn = false;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        await NpCharge.Gain(Owner, RainWitchFormPower.NpPerTurn, null);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (dealer != Owner || target.IsPlayer || target.IsDead) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        if (!_npThisTurn)
        {
            _npThisTurn = true;
            Flash();
            await NpCharge.Gain(Owner, FairyQueenFormPower.NpOnDamage, null);
        }

        // Sentencia (igual que Berserker): detoná la Maldición del objetivo.
        var curse = Curses.Of(target);
        if (curse <= 0) return;
        var consumed = await Curses.Consume(target, curse);
        if (consumed <= 0) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, target, consumed,
            ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner, null);
    }
}
