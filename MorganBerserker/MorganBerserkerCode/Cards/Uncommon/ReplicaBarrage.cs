using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Andanada de Réplicas (复制枪连射) — 5 de daño ×3; Carga NP +5 por golpe que dañe HP.</summary>
public sealed class ReplicaBarrage() : MorganCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int Hits = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("NpCharge", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
        var hpHits = attack.Results.Count(r => r.UnblockedDamage > 0);
        if (hpHits > 0)
        {
            await NpCharge.Gain(Owner.Creature, hpHits * DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
