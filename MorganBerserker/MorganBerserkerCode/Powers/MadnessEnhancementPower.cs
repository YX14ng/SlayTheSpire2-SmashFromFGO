using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Refuerzo de Locura (狂化) — whenever you lose HP during your turn: NP Charge
/// +Amount (capped at 2 triggers per turn — el fix del panel a los grifos de NP).
/// </summary>
public sealed class MadnessEnhancementPower : MorganPower
{
    public const int TriggersPerTurn = 2;

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
        if (_triggersThisTurn >= TriggersPerTurn) return;

        _triggersThisTurn++;
        Flash();
        await NpCharge.Gain(Owner, Amount, null);
    }
}
