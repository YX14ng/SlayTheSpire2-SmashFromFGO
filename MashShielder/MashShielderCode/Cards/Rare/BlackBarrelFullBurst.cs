using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// BLACK BARREL — Disparo Pleno. NP card (50 charge): huge Black Barrel shot.
/// Overcharge (cast at exactly 100): strips ALL the target's buffs.
/// </summary>
public sealed class BlackBarrelFullBurst() : MashShielderCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int ChargeCost = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(35m, ValueProp.Move | ValueProp.Unblockable),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var overcharged = NpCharge.IsOvercharged(Owner.Creature);
        await NpCharge.PayForNpCard(Owner.Creature, ChargeCost, this);

        await BlackBarrel.Hit(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, Owner.Creature, this);
        if (overcharged && !cardPlay.Target.IsDead)
        {
            await BlackBarrel.RemoveAllBuffs(cardPlay.Target);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}
