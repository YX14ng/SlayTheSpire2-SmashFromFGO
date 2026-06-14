using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Cards.Special;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Cerezo en Plena Floración (樱花满开) — DESIGN-OKITA §5.4. 1⚡ Hab, Exhaust: tu *Aliento se llena al
/// máximo; ganás 1 *Tos (al mazo) (up: sin Tos). El enabler del turno de gloria definitivo. La mejora
/// quita el downside (gate por IsUpgraded).
/// </summary>
public sealed class CherryInFullBloom() : OkitaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Aliento.FillToCap(Owner.Creature, this);
        if (!IsUpgraded) await Tos.ShuffleIntoDraw(Owner.Creature, this);
    }
}
