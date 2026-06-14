using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// PODER «Recuerdo de la Última Primavera» (最后春日之忆) — DESIGN-OKITA §5.4. 2⚡ Poder: aplica
/// <see cref="LastSpringMemoryPower"/> con Amount 20 (al inicio de cada turno: +20★) (up: +30). El
/// motor ★ per-turn (la paz del engawa).
/// </summary>
public sealed class LastSpringMemory() : OkitaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<LastSpringMemoryPower>("Stars", 20m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LastSpringMemoryPower>(Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m); // +20 -> +30
}
