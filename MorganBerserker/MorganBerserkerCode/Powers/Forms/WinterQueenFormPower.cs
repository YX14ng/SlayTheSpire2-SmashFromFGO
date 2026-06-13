using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Reina del Invierno (冬之女王) — forma clímax PERMANENTE: ambas a la vez sin penalidad.
/// GENERA estrellas como Caster Y es donde DETONÁS sin el -2 daño (rediseño 2026-06-13).
/// (a) Al inicio de tu turno: +12 Estrellas de Crítico. (b) Primera vez que dañás HP
/// enemigo cada turno: +10 NP. Sin penalización de Ataque. Es la meta aspiracional del mazo.
/// </summary>
public sealed class WinterQueenFormPower : MorganFormPower
{
    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_winter.tres";

    public override bool IsPermanent => true;

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
        await CritStars.Gain(Owner, RainWitchFormPower.StarsPerTurn, null);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (_npThisTurn || dealer != Owner || target.IsPlayer) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        _npThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, FairyQueenFormPower.NpOnDamage, null);
    }
}
