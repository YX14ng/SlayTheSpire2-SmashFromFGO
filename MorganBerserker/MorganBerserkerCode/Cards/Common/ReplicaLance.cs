using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Réplica de Lanza (长枪复制品) — re-spec RE-POOL V2 (parche J3-9, injerto de P2): 4 de daño ×2,
/// 1 de Maldición por golpe (mejora 6×2, 2 por golpe). El multi-hit anti-Buffer en COMÚN que el
/// pool no tenía; las réplicas de Rhongomyniad hexan al clavarse.
/// </summary>
public sealed class ReplicaLance() : MorganCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int Hits = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("Curse", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
        if (!cardPlay.Target.IsDead)
        {
            await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue * Hits, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
