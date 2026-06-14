using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Filo Centelleante (闪光之刃) — DESIGN-OKITA §5.3. 1⚡ At: 6 daño; +20★ (up: 9 / +30★). Slot Baobhan.
/// </summary>
public sealed class GlintingEdge() : OkitaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Stars", 20)];

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
        DynamicVars.Damage.UpgradeValueBy(3m);    // 6 -> 9
        DynamicVars["Stars"].UpgradeValueBy(10m); // +20 -> +30
    }
}
