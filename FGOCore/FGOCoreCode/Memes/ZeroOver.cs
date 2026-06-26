using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Limited/Zero Over (无限剑制) — ESTE turno, tus Ataques hacen +30% de daño
/// (<see cref="ZeroOverPower"/>, se auto-remueve al fin de turno). Habilitador de burst para
/// un turno de NP/Ataques gordos. Sin daño propio, 0⚡ + Exhaust.
/// </summary>
public sealed class ZeroOver() : MemeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // Sin var numérica: el +30% es fijo (ZeroOverPower.DamageMultiplier) y va literal en la loc.
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ZeroOverPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ZeroOverPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
