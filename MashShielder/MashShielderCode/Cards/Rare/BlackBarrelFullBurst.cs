using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// BLACK BARREL — Disparo Pleno. NP card (min 50 charge, consumes ALL): huge Black Barrel shot.
/// FGO Overcharge: +damage per 10 extra charge; at a full 100 it strips ALL the target's buffs.
/// </summary>
public sealed class BlackBarrelFullBurst() : MashShielderCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int ChargeCost = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(35m, ValueProp.Move | ValueProp.Unblockable),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        await BlackBarrel.Hit(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue + bonus, Owner.Creature, this);
        if (tier >= NpChargePower.Max && !cardPlay.Target.IsDead)
        {
            await BlackBarrel.RemoveAllBuffs(cardPlay.Target);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
        DynamicVars["PerTen"].UpgradeValueBy(1m);
    }
}
