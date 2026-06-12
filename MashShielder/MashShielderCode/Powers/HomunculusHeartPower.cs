using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Corazón del Homúnculo — whenever Mash changes form: draw cards and gain NP Charge.
/// Stacks scale the effect (1 stack: draw 2, NP +10; 2 stacks: draw 3, NP +20).
/// Parche P2 del rediseño v2: máximo 2 procs por turno (corta el loop de robo
/// FormDrill × HomunculusHeart a 0E; el ping-pong de formas sigue, regla §5).
/// Notified by FGOCore's FormSwitch via IFormChangeListener.
/// </summary>
public sealed class HomunculusHeartPower : MashShielderPower, IFormChangeListener
{
    public const int MaxProcsPerTurn = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int _procsThisTurn;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _procsThisTurn = 0;
        return Task.CompletedTask;
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_procsThisTurn >= MaxProcsPerTurn) return;
        _procsThisTurn++;
        Flash();
        if (choiceContext != null && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, 1 + Amount, Owner.Player);
        }
        await NpCharge.Gain(Owner, 10 * Amount, null);
    }
}
