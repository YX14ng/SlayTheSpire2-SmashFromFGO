using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Lupa de la Detective del Verano — rare: your Criticals deal +2 damage and grant
/// NP Charge +3 when the stars are consumed (sews ★→NP: cashing stars also charges
/// the ult, fixing the «always cross 100 in Caster» bias).
/// </summary>
public sealed class DetectiveMagnifier : ArtoriaRelic, ICritListenerWithContext, ICritDamageBoost, ICriticalConsumedListener
{
    public const int Bonus = 2;
    public const int NpPerCrit = 3;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public int CritDamageBonus => Bonus;

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner.Creature || !props.IsPoweredAttack() || cardSource == null) return 0m;
        return Criticals.WillCrit(Owner.Creature, cardSource) ? Bonus : 0m;
    }

    public async Task AfterCritConsumed(int starsSpent)
    {
        await AfterCritConsumed(new BlockingPlayerChoiceContext(), starsSpent);
    }

    public async Task AfterCritConsumed(PlayerChoiceContext choiceContext, int starsSpent)
    {
        Flash();
        await NpCharge.Gain(choiceContext, Owner.Creature, NpPerCrit, null);
    }

    public Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical) =>
        critical.Owner == Owner.Creature
            ? AfterCritConsumed(context, critical.StarsSpent)
            : Task.CompletedTask;
}
