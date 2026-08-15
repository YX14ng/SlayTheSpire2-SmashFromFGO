using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Marea de Estrellas — ganás 3★. Mejorada: 4★. Rescate P2 2026-06-25: se le sacó el *Exhaust
/// (era un setup de un solo uso dominado por los generadores con rider); ahora es un grifo de ★
/// repetible, el ancla del arquetipo crítico.
/// </summary>
public sealed class StarTide() : ArtoriaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // Rebalance 2026-08-15 (REBALANCE-TIAMAT-ARTORIA.md A5): 30→40 — un crítico cuesta 50★ y 30
    // no llegaba ni al 60%; DESIGN-ARTORIA §8.bis ya pedía generadores +25%.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 40)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Stars.Gain(choiceContext, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(20m);
    }
}
