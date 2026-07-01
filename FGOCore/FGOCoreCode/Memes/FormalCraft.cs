using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Np;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Maquinaria Mística (魔术礼装) — ESTE turno, tus cartas que cargan NP cargan +30%
/// (<see cref="FormalCraftPower"/>, se auto-remueve al fin de turno). Pieza de combo barata:
/// jugala antes de un turno de generación para inflar toda la carga. Sin daño, 0⚡ + Exhaust.
/// </summary>
public sealed class FormalCraft() : MemeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), INpDependentColorless
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // Sin var numérica: el +30% es fijo (FormalCraftPower.Ratio) y va literal en la loc.
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<FormalCraftPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FormalCraftPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
