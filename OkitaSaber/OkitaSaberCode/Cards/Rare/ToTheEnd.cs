using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Hasta el Final (直到最后) — DESIGN-OKITA §5.4. 0⚡ Hab, Exhaust: aplica <see cref="ToTheEndPower"/>
/// (este turno tus RÁFAGAS no cuestan *Aliento; en su lugar pagás 2 HP por cada punto que hubieras
/// pagado) (up: 1 HP/punto). El override: el plan nunca muere, el cuerpo paga. Fija el HpCostPerPoint
/// del power según la mejora.
/// </summary>
public sealed class ToTheEnd() : OkitaCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ToTheEndPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.HpCostPerPoint = IsUpgraded ? 1 : 2;
    }
}
