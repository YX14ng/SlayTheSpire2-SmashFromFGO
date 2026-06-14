using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Acción Independiente A+ (单独行动 A+) — la pasiva real (Crit Damage). 1⚡ Poder: aplica
/// <see cref="IndependentActionPower"/> con Amount 6 (tus Ataques con Crítico Listo hacen +6 antes del
/// ×2). up 6→9.</summary>
public sealed class KitIndependentAction() : GilgameshCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<IndependentActionPower>("Action", 6m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IndependentActionPower>(Owner.Creature, DynamicVars["Action"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Action"].UpgradeValueBy(3m);
}
