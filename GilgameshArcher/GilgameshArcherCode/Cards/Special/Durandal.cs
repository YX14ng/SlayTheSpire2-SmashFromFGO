using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// Durandal, la Indestructible / 不毁之剑·杜兰达尔 (DESIGN-GILGAMESH §5.5, Arma del Tesoro #5) — el original
/// de la espada que nunca se rompe; alimenta el hilo de las Estrellas. Token 0⚡ Exhaust generado
/// (<c>CardRarity.Event</c>). 6 de daño + 10 Estrellas de Crítico (reusa <c>CritStars</c> de FGOCore).
/// </summary>
public sealed class Durandal() : GilgameshCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    private const int StarsGain = 10;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Stars", StarsGain)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await ArmsPlayedPower.Record(Owner.Creature);
    }
}
