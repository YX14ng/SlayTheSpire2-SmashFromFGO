using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>
/// Descarga del Cilindro — X cost: each ⚡ also drains 10 NP Charge and deals damage.
/// Hits = min(X, affordable NP increments).
/// </summary>
public sealed class CylinderDischarge() : MashShielderCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int ChargePerHit = 10;

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var x = ResolveEnergyXValue();
        var hits = Math.Min(x, NpCharge.Current(Owner.Creature) / ChargePerHit);
        if (hits <= 0) return;

        await NpCharge.Spend(Owner.Creature, hits * ChargePerHit, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hits).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_pierce")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
