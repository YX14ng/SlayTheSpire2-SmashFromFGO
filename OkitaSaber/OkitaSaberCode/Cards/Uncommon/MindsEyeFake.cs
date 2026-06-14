using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Ojo Mental (Falso) A (心眼（伪）A, KIT) — DESIGN-OKITA §5.3. 1⚡ Hab, Exhaust: +1 Intangible
/// (up: y +20★). El Evade 1T del skill real — su única defensa real.
/// </summary>
public sealed class MindsEyeFake() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<IntangiblePower>("Intangible", 1m), new DynamicVar("Stars", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IntangiblePower>(Owner.Creature, DynamicVars["Intangible"].BaseValue, Owner.Creature, this);
        var stars = DynamicVars["Stars"].IntValue;
        if (stars > 0) await CritStars.Gain(Owner.Creature, stars, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(20m); // 0 -> +20★
}
