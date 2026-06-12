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
/// Réplica de Rhongomyniad — NP card (min 100 charge, consumes ALL): the lance that anchors
/// the world. FGO Overcharge: +damage per 10 extra charge. Exhaust.
/// Rediseño v2 + parche P6: mínimo 70 → 100; 39 daño BB (up +10 = 49) y PerTen 5 (up +1 = 6),
/// presupuestados ASUMIENDO CritReady ×2 como caso de uso normal — el ×2 SÍ aplica al daño
/// Black Barrel/Unblockable (decisión, no accidente: CritReadyPower no distingue).
/// A banco 300 con up: (49 + 20×6) × 2 = 338, el all-in legítimo del hilo de estrellas.
/// </summary>
public sealed class RhongomyniadReplica() : MashShielderCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IMashNpCard
{
    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(39m, ValueProp.Move | ValueProp.Unblockable),
        new PowerVar<VulnerablePower>("Vulnerable", 3m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

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
        DynamicVars.Damage.UpgradeValueBy(10m);
        DynamicVars["PerTen"].UpgradeValueBy(1m);
    }
}
