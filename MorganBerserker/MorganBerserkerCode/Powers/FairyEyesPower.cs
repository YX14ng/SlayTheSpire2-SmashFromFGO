using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>Ojos Feéricos (妖精眼) — at the start of your turn: apply Amount Curse to ALL enemies.</summary>
public sealed class FairyEyesPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        foreach (var enemy in Owner.CombatState.GetOpponentsOf(Owner))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(enemy, Amount, Owner, null);
            }
        }
    }
}
