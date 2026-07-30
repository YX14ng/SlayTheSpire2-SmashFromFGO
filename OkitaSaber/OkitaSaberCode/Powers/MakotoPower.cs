using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Makoto (誠) — el estandarte (DESIGN-OKITA §5.4). Cada vez que tu umbral de 100★ se activa
/// (ganás un *Crítico Listo por auto-payoff), tus Ataques hacen +<see cref="PerActivation"/>
/// este combate, con tope de <see cref="MaxBonus"/> (up: <see cref="MaxBonusUpgraded"/>).
///
/// Detección sin API nueva: el auto-payoff de CritStarsPower a 100 aplica CritReadyPower; lo
/// leemos en AfterPowerAmountChanged (amount > 0 = un Crítico Listo GANADO = un umbral cumplido),
/// idéntico al patrón de CombatAnalysisPower de Mash.
///
/// Single (Amount = 1 marcador): el bono acumulado vive en <see cref="Bonus"/>, NO en Amount,
/// porque PowerCmd.Apply retorna temprano con amount 0 y un Counter en 0 se auto-remueve. Patrón
/// de ToTheEndPower/SteadyStepPower (Single + campo settable). Personal: no escala en multijugador.
/// </summary>
public sealed class MakotoPower : OkitaPower, ICriticalConsumedListener
{
    public const int PerActivation = 2;
    public const int MaxBonus = 10;
    public const int MaxBonusUpgraded = 16;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>Tope del bono acumulado (la carta lo sube a 16 al mejorarse).</summary>
    public int Cap { get; set; } = MaxBonus;

    /// <summary>Bono aditivo acumulado actual (lo que suman tus Ataques). Vive aquí, no en Amount.</summary>
    public int Bonus { get; private set; }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        return Bonus;
    }

    public Task AfterCriticalConsumed(PlayerChoiceContext choiceContext, CriticalHit critical)
    {
        if (critical.Owner != Owner || Bonus >= Cap) return Task.CompletedTask;
        Flash();
        Bonus = Math.Min(Cap, Bonus + PerActivation);
        return Task.CompletedTask;
    }
}
