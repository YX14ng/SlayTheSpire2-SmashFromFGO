using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// Hárpē, la Hoz Inmortal / 不死斩·哈尔贝 (DESIGN-GILGAMESH §5.5, Arma del Tesoro #3) — la hoz que niega
/// la inmortalidad. Token 0⚡ Exhaust generado (<c>CardRarity.Event</c>). 5 de daño y aplica 1 de Débil.
/// </summary>
public sealed class Harpe() : GilgameshCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new PowerVar<WeakPower>("Weak", 1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);
        if (!cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        }
        await ArmsPlayedPower.Record(Owner.Creature);
    }
}
