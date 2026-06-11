using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Pacto de Sangre Feérica (妖精血之契约) — at the start of your turn: lose 2 HP
/// and gain Amount NP Charge.
/// </summary>
public sealed class FaeBloodPactPower : MorganPower
{
    public const int HpCost = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, HpCost,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, null, null);
        if (Owner.IsAlive)
        {
            await NpCharge.Gain(Owner, Amount, null);
        }
    }
}
