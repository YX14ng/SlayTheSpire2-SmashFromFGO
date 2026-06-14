using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Suerte EX (幸运EX / Luck EX) — DESIGN-OBERON §6.4. 1⚡ Poder: al inicio de tu turno, +20 Estrellas
/// (up +30). La star gen 20,5% real de Oberon hecha motor. Aplica <see cref="LuckExPower"/> y le fija el
/// rider desde el DynamicVar.
/// </summary>
public sealed class LuckEX() : OberonCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<LuckExPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.Stars = DynamicVars["Stars"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
