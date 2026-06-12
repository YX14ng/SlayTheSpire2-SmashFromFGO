using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// «Un Hogar con Morgan» (想和摩根组建家庭) — +3 HP máximos y cura 3 HP; si tienes
/// Alzarse: ambos se duplican (un hogar = algo por lo que vivir). Exhaust.
/// Rediseño v2: lector explícito 1 del hilo Alzarse + glow dorado. (El meme CN del pool.)
/// </summary>
public sealed class AHomeWithMorgan() : MorganCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(3m),
        new HealVar(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GutsPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<GutsPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var multiplier = Owner.Creature.HasPower<GutsPower>() ? 2m : 1m;
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue * multiplier);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue * multiplier);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.MaxHp.UpgradeValueBy(2m);
        DynamicVars.Heal.UpgradeValueBy(2m);
    }
}
