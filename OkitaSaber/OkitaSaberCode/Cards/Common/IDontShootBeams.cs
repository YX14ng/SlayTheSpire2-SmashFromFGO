using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// «¡No disparo rayos!» (我才不会射光线, meme canon Saberface) — DESIGN-OKITA §5.2. 0⚡ At, Exhaust:
/// 3 daño; +20★ (el destello NO era un rayo) (up: 5 / +30★). El gag Saberface.
/// </summary>
public sealed class IDontShootBeams() : OkitaCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);   // 3 -> 5
        DynamicVars["Stars"].UpgradeValueBy(10m); // +20 -> +30
    }
}
