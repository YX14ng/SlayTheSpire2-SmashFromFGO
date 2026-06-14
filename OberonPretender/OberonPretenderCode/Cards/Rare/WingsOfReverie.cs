using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Alas del Ensueño (梦想之翼 / Wings of Reverie) — DESIGN-OBERON §6.4. 2⚡ Poder: cuando jugás una
/// carta NP, +20 Estrellas y robá 1 (up +30). Los finishers realimentan la economía. Aplica
/// <see cref="WingsOfReveriePower"/> y le fija el rider desde el DynamicVar.
/// </summary>
public sealed class WingsOfReverie() : OberonCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<WingsOfReveriePower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.Stars = DynamicVars["Stars"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
