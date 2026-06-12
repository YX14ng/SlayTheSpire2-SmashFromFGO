using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>
/// Descarga del Cilindro — costs no Energy: consumes ALL NP Charge (in 10s)
/// and deals one hit per 10 consumed. The leftover (&lt;10) is kept.
/// Rediseño v2 (AUDITORÍA): 4 de daño por cada 10 consumidos (up 5; antes 10 —
/// la conversión 1:1 NP→daño rompía la tasa). A 300 = 120/150: dump all-in.
/// </summary>
public sealed class CylinderDischarge() : MashShielderCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int ChargePerHit = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargePerHit);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var hits = NpCharge.Current(Owner.Creature) / ChargePerHit;
        if (hits <= 0) return;

        await NpCharge.Spend(Owner.Creature, hits * ChargePerHit, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hits).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
