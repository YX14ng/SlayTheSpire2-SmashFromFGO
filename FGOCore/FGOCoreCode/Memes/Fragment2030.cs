using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Stars;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Un Fragmento de 2030 (2030年的碎片) — Poder: al inicio de cada turno ganás 3 Estrellas de
/// Crítico. Motor de generación pasiva de estrellas (~3★ ≈ 9-15 daño/turno de payoff a 100★).
/// </summary>
public sealed class Fragment2030() : MemeCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<Fragment2030Power>("Stars", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Fragment2030Power>(choiceContext, Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(1m);
    }
}
