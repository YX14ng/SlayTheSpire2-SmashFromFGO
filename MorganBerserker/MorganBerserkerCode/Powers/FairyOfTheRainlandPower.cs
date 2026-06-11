using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>Hada del País de la Lluvia (雨之国的妖精) — at the start of your turn: NP Charge +Amount.</summary>
public sealed class FairyOfTheRainlandPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await NpCharge.Gain(Owner, Amount, null);
    }
}
