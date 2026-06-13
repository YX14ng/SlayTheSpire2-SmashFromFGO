using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// La Reina (Berserker, 妖精女王) — forma inicial. El DETONADOR del motor Buster
/// (rediseño 2026-06-13): es donde volcás el banco de Estrellas en Ataques Buster con
/// "Crítico" (gastá 50★ → ×2). Genera poco recurso propio → te empuja a alternar con
/// Caster para re-llenar el banco. La primera vez que dañás HP enemigo cada turno: +10 NP.
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

        if (_npThisTurn || dealer != Owner || target.IsPlayer) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        _npThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, NpOnDamage, null);
    }
}
