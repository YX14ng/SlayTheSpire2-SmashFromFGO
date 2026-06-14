using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Odio Acumulado (积怨, §5.2) — cada vez que perdés Vida ganás <see cref="NpPerLoss"/> de Carga NP
/// (10), con tope <see cref="MaxProcsPerTurn"/> activaciones/turno (2; up 3). DESIGN-MORDRED §5.2:
/// cada sangrado paga DOS economías con la starter (la starter da ★ por pérdida de Vida, este
/// poder da NP) — el odio se recicla. Reacciona a CUALQUIER pérdida de Vida (golpe enemigo o
/// auto-daño de cartas como Mandoble Orgulloso/Berrinche Real), espejo de la conversión de la
/// reliquia «Clarent, la Espada Robada» (AfterCurrentHpChanged + cap por turno + reset al inicio).
/// El tope es campo settable que fija la carta desde su DynamicVar; Amount es el conteo de stacks.
/// </summary>
public sealed class AccumulatedHatredPower : MordredPower
{
    public const int NpPerLoss = 10;

    public int MaxProcsPerTurn = 2; // up 3

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private int _procsThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) _procsThisTurn = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner || delta >= 0) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (_procsThisTurn >= MaxProcsPerTurn) return;
        _procsThisTurn++;
        Flash();
        await NpCharge.Gain(Owner, NpPerLoss, null);
    }
}
