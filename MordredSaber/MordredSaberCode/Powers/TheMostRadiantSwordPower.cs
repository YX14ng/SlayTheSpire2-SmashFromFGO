using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// La Espada Más Resplandeciente (最耀眼之剑, §5.3) — Clarent que cose ★→NP: el premio del motor.
///   - tus CRÍTICOS hacen +<see cref="CritBonus"/> ADICIONAL (el +N entra al golpe que va a
///     consumir el *Crítico Listo, ANTES del ×2 — se dobla con él, igual que KnightOfRedLightningPower);
///   - cada vez que CONSUMÍS un Crítico, ganás +<see cref="NpOnConsume"/> NP EXTRA
///     (vía <see cref="ICritConsumedListener"/>, lo dispara RedLightningChannelPower — encima del
///     +10 NP de la starter).
/// El <see cref="CritBonus"/> y el <see cref="NpOnConsume"/> son campos settables que fija la carta
/// desde sus DynamicVars (para que el up se refleje sin chocar con el conteo de stacks). Counter:
/// copias suman el daño-crit; el +NP/consumo no apila (un solo broadcast por consumo). Personal.
/// </summary>
public sealed class TheMostRadiantSwordPower : MordredPower, ICritConsumedListener
{
    public int CritBonus = 8;     // up 12 (la carta lo setea desde su DynamicVar)
    public int NpOnConsume = 10;  // +NP extra al consumir un crítico

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        // +Crítico extra solo cuando hay un Crítico Listo en cola (el golpe que se va a doblar).
        return Owner.GetPowerAmount<CritReadyPower>() > 0 ? CritBonus * (int)Amount : 0m;
    }

    public async Task OnCritConsumed(PlayerChoiceContext? choiceContext)
    {
        Flash();
        await NpCharge.Gain(Owner, NpOnConsume, null);
    }
}
