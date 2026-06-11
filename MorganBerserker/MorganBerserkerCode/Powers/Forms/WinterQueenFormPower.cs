using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Reina del Invierno (冬之女王) — permanent climax form (rare card only):
/// both reigns at once. (a) First HP damage each turn: NP +5. (b) Curse cards
/// apply +1. (c) At the start of your turn: NP +5. No attack penalty.
/// The manifested ult is the Queen's (Roadless Camelot).
/// </summary>
public sealed class WinterQueenFormPower : MorganFormPower, ICurseAmplifier
{
    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_winter.tres";

    public override bool IsPermanent => true;

    public int ExtraCurse => 1;

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

        if (_npThisTurn || dealer != Owner || target.IsPlayer) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        _npThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, FairyQueenFormPower.NpOnDamage, null);
    }
}
