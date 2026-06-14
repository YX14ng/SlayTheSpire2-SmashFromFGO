using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Lectura Nocturna (Night Reading) -- DESIGN-OBERON 6.3 (P10 Morgan: la velocidad de mazo es
/// el combustible del banco). Al inicio de tu turno: roba <see cref="Amount"/> cartas adicionales
/// (Counter: una 2a copia roba +1). El up baja el coste de la carta, no el power.
/// </summary>
public sealed class NightReadingPower : OberonPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), Amount, Owner.Player);
    }
}
