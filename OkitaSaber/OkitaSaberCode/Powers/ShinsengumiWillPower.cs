using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Voluntad del Shinsengumi (新选组之志) — al final de tu turno, si jugaste <see cref="AttacksRequired"/>+
/// Ataques: +<see cref="Block"/> Bloqueo y +<see cref="NpGainValue"/> Carga NP (DESIGN-OKITA §5.3,
/// arquetipo Shukuchi defensivo). Flag privado de conteo (patrón WeightOfExpectationsPower).
/// </summary>
public sealed class ShinsengumiWillPower : OkitaPower
{
    public const int AttacksRequired = 3;
    public const int Block = 5;
    public const int NpGainValue = 20;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private int _attacksThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) _attacksThisTurn = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner) _attacksThisTurn++;
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || _attacksThisTurn < AttacksRequired) return;
        Flash();
        for (var i = 0; i < Amount; i++)
        {
            await CreatureCmd.GainBlock(Owner, Block, ValueProp.Move, null, false);
            await NpCharge.Gain(Owner, NpGainValue, null);
        }
    }
}
