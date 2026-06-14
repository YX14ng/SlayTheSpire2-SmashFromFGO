using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Danza de la Hoja (刃之舞) — DESIGN-OKITA §5.3. 1⚡ At: 4 daño ×2; +10★ (up: 6×2 / +20★). Multi-hit ★.
/// </summary>
public sealed class BladeDance() : OkitaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int Hits = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        for (var i = 0; i < Hits; i++)
        {
            if (cardPlay.Target.IsDead) break;
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);   // 4 -> 6
        DynamicVars["Stars"].UpgradeValueBy(10m); // +10 -> +20
    }
}
