using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>Gram, el Original de Todas las Espadas (始源之剑·格拉墨) — DESIGN-GILGAMESH §5.4, slot Bludgeon.
/// 3⚡ At: 30 de daño single-hit + 10 Estrellas de Crítico (up 38 / +20). El blanco soñado del Crítico ×2
/// (60-76). El up sube ambos.</summary>
public sealed class GramOriginOfAllSwords() : GilgameshCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(30m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
