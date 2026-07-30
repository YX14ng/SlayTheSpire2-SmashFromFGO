using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>
/// Espira de Leviatán — el PICO de daño de la fase Lily (DESIGN-REVIEW-2 §2): un golpe grande que
/// siembra Maldición y, si la presa ya estaba bien maldita (!Threshold!+), la espira la enrosca DOS
/// veces (golpe doble). El payoff que recompensa haber montado el campo de Maldición, sin necesidad
/// de la ventana Bestia.
/// </summary>
public sealed class LeviathanCoil() : TiamatCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new DynamicVar("Curse", 4),
        new DynamicVar("Threshold", 8)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // El segundo golpe se decide ANTES de aplicar la Maldición de esta carta (lee el campo previo).
        var doubleStrike = Curses.Of(cardPlay.Target) >= DynamicVars["Threshold"].IntValue;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
        if (doubleStrike && cardPlay.Target.IsAlive)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_bloody_impact")
                .Execute(choiceContext);
        }
        if (cardPlay.Target.IsAlive)
        {
            await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
