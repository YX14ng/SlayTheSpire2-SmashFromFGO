using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Réplica de Rhongomyniad — NP card (min 70 charge, consumes ALL): the lance that anchors
/// the world. FGO Overcharge: +damage per 10 extra charge. Exhaust.
/// </summary>
public sealed class RhongomyniadReplica() : MashShielderCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int ChargeCost = 70;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(39m, ValueProp.Move | ValueProp.Unblockable),
        new PowerVar<VulnerablePower>("Vulnerable", 3m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 7)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + bonus);

        await BlackBarrel.Hit(choiceContext, cardPlay.Target, damage, Owner.Creature, this);
        if (!cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(15m);
        DynamicVars["PerTen"].UpgradeValueBy(2m);
    }
}
