using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    public async Task OnCritConsumed(PlayerChoiceContext? choiceContext)
    {
        if (FgoCombatState.GetTurn(Owner, 2) != 0 || Owner.IsDead) return;
        var context = choiceContext ?? new BlockingPlayerChoiceContext();
        await FgoCombatState.SetTurn(context, Owner, 2, 1);
        Flash();
        await CritStars.Gain(context, Owner, StarsPerTurn * (int)Amount, null);
    }
}
