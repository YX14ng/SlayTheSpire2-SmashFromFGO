using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Aceleración de Homúnculo (人造人加速, §5.2) — la 1ª vez por turno que CONSUMÍS un *Crítico Listo*
/// ganás <see cref="StarsPerTurn"/> Estrellas (10; up 20). Implementa <see cref="ICritConsumedListener"/>
/// (lo dispara <see cref="RedLightningChannelPower"/> al detectar el consumo). El cierre del motor
/// ★→×2→NP→★, CAPEADO a 1 activación/turno (P3: reset del flag al inicio de tu turno). El valor por
/// activación es campo settable que fija la carta desde su DynamicVar; Amount es el conteo de stacks.
/// </summary>
public sealed class HomunculusAccelerationPower : MordredPower, ICritConsumedListener
{
    public int StarsPerTurn = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _firedThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) _firedThisTurn = false;
        return Task.CompletedTask;
    }

    public async Task OnCritConsumed(PlayerChoiceContext? choiceContext)
    {
        if (_firedThisTurn || Owner.IsDead) return;
        _firedThisTurn = true;
        Flash();
        await CritStars.Gain(Owner, StarsPerTurn * (int)Amount, null);
    }
}
