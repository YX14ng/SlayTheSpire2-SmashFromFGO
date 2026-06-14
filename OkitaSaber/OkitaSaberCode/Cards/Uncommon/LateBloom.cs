using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Florecer Tardío (迟开之花, KIT) — DESIGN-OKITA §5.3. 1⚡ Poder: aplica <see cref="LateBloomPower"/>
/// (cada vez que ganás una *Tos: +20★) (up: +30★). La enfermedad alimenta la gloria. El Amount del
/// power (Counter) son las ★/Tos.
/// </summary>
public sealed class LateBloom() : OkitaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<LateBloomPower>("Stars", 20m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LateBloomPower>(Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m); // +20★ -> +30★
}
