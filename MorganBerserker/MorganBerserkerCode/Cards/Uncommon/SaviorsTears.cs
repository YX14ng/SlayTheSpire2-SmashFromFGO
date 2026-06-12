using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Lágrimas de la Salvadora (救世妖精之泪) — cura 5 HP; si tu HP ≤ 50%: cura 9. Exhaust.
/// Rediseño v2: glow dorado a ≤50% (umbral de Vida alcanzado).
/// </summary>
public sealed class SaviorsTears() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(5m),
        new DynamicVar("BigHeal", 9)
    ];

    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.CurrentHp * 2 <= Owner.Creature.MaxHp;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var lowHp = Owner.Creature.CurrentHp * 2 <= Owner.Creature.MaxHp;
        await CreatureCmd.Heal(Owner.Creature,
            lowHp ? DynamicVars["BigHeal"].BaseValue : DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2m);
        DynamicVars["BigHeal"].UpgradeValueBy(3m);
    }
}
