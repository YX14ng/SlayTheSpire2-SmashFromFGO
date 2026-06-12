using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Refuerzo de Locura (狂化) — whenever you lose HP during your turn: NP Charge
/// +10 (denominación fija "rider"), capped at Amount triggers per turn.
/// Rediseño v2: el monto pasa a ser fijo (6→10) y el upgrade sube el TOPE de
/// activaciones (2→3) en vez del monto — Amount = máx. triggers/turno.
/// Parche P4: el tick de FaeBloodPact NO dispara este power (el Pacto ya se paga
/// solo con su propio NP; sin el filtro además quemaba involuntariamente
/// 1 de los triggers/turno que el jugador quería para su MadLunge).
/// </summary>
public sealed class MadnessEnhancementPower : MorganPower
{
    public const int NpPerTrigger = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _isPlayerTurn;
    private int _triggersThisTurn;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        _isPlayerTurn = side == CombatSide.Player;
        if (_isPlayerTurn) _triggersThisTurn = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || !_isPlayerTurn || result.UnblockedDamage <= 0) return;
        if (FaeBloodPactPower.TickInProgress) return; // P4: el tick del Pacto no cuenta.
        if (_triggersThisTurn >= Amount) return;

        _triggersThisTurn++;
        Flash();
        await NpCharge.Gain(Owner, NpPerTrigger, null);
    }
}
