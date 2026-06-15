using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>Lluvia de Mil Armas (千刃之雨) — DESIGN-GILGAMESH §5.4, slot Comet (50★ ≈ 5★ vanilla; gastar
/// retrasa el auto-Crítico = tensión real). 0⚡ At: jugable SÓLO con ≥50 Estrellas de Crítico; consume 50;
/// 25 de daño single (up 32). Glow cuando pagable. El up sube SOLO el daño; el costo queda en 50. Patrón
/// CollectorsGreed (IsPlayable = CritStars.CanPay, early-return, CritStars.Gain(-costo)).</summary>
public sealed class RainOfAThousandBlades() : GilgameshCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StarCost", 50), new DamageVar(25m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (!CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue)) return;
        await CritStars.Gain(Owner.Creature, -DynamicVars["StarCost"].IntValue, this);
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(7m);
}
