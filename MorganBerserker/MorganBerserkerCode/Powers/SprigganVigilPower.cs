using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Vigilia de Spriggan (斯普里根的看守) — at the start of your turn: gain Amount
/// Block and NP Charge +5 (rediseño v2: Spriggan custodia el tesoro de la Reina —
/// el Bloqueo ahora alimenta el hilo NP; deja de ser isla).
/// </summary>
public sealed class SprigganVigilPower : MorganPower
{
    public const int NpPerTurn = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        await NpCharge.Gain(Owner, NpPerTurn, null);
    }
}
