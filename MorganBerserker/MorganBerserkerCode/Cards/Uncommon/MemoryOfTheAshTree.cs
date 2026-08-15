using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Memoria del Fresno (梣树之忆) — re-efecto RE-POOL V2 (válvula de P2, parche J2-7): 0⚡, si
/// tenés ≥30 de Carga NP: gastá 30 y robá 2 (mejora: robá 3). LA ÚNICA arista NP→cartas del
/// grafo (§13); repetible sin Agotar porque GASTA un recurso real (riesgo 5, vigilado). Patrón
/// de gate/glow calcado de Velo de Niebla (carga real, sin waiver).
/// </summary>
public sealed class MemoryOfTheAshTree() : MorganCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ChargeCost", 30),
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["ChargeCost"].IntValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await NpCharge.Spend(choiceContext, Owner.Creature, DynamicVars["ChargeCost"].IntValue, this)) return;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
