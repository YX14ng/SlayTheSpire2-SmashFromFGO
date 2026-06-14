using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Plan del Final Feliz (Happy Ending Plan) -- DESIGN-OBERON 6.4. Al inicio de cada
/// turno: +<see cref="PerTurn"/> de Bendicion de Sobrecarga (<see cref="OverchargeBlessingPower"/>),
/// empujando los ultis a >=150 -> el sueno masivo. El +2 de entrada lo da la carta al jugarla.
/// </summary>
public sealed class HappyEndingPlanPower : OberonPower
{
    public const int PerTurn = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await PowerCmd.Apply<OverchargeBlessingPower>(Owner, PerTurn, Owner, null);
    }
}
