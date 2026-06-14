using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Instinto de Batalla (战斗直觉) — DESIGN-MORDRED §5.1. 1⚡ Hab: si tenés ≥30 Estrellas, perdé 30 y
/// robá 2 (up: consume solo 20), glow. El Instinct B en común: las ★ se gastan por cartas. Patrón
/// CylinderSeal (gasto de ★) + RoyalEdict (robo). El robo NO sube con el up (solo baja el costo).
/// </summary>
public sealed class BattleInstinct() : MordredCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 30), new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue)) return;
        await CritStars.Gain(Owner.Creature, -DynamicVars["StarCost"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["StarCost"].UpgradeValueBy(-10m);
}
