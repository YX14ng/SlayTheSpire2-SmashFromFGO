using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Creación de Territorio (阵地作成) — at the end of your turn: gain Amount Block
/// + 5 NP Charge (rediseño v2: el territorio canaliza maná — era isla de Bloqueo
/// puro; el NP es fijo por instancia, solo el Bloqueo escala con stacks).
/// </summary>
public sealed class TerritoryCreationPower : MorganPower
{
    public const int NpPerTurn = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        await NpCharge.Gain(Owner, NpPerTurn, null);
    }
}
