using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Mirada del Prodigio (天才之瞳) — DESIGN-OKITA §5.2. 0⚡ Hab: aplica 1 Débil a UN enemigo; +10★
/// (up: +20★). Control suave + ★.
/// </summary>
public sealed class ProdigysGlare() : OkitaCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m); // +10 -> +20
}
