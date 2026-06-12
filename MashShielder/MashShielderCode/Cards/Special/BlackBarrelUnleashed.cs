using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Special;

/// <summary>
/// The ult manifested at 100 NP while in ORTINAX form — the Black Barrel, the
/// conceptual cannon of the Atlas Institute that kills the immortal: a massive
/// unblockable shot that strips ALL the target's buffs.
/// </summary>
public sealed class BlackBarrelUnleashed() : MashShielderCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy), IMashNpCard
{
    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(32m, ValueProp.Move | ValueProp.Unblockable),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    // Pasar la carta: el waiver de Pioneer NO cubre Event (parche P3) — sin él,
    // CanPay daría glow/playable falsos con el medidor vacío y un waiver activo.
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
            await BlackBarrel.RemoveAllBuffs(cardPlay.Target);
        }
    }
}
