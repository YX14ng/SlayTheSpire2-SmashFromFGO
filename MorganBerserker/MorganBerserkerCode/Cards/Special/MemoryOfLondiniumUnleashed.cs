using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Special;

/// <summary>
/// The ult manifested at 100 NP in Rain Witch form: MEMORY OF LONDINIUM unleashed —
/// AoE + Intangible + Knight's Arms, free, Exhaust. At 200+ consumed: +1 Arm.
/// </summary>
public sealed class MemoryOfLondiniumUnleashed() : MorganCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies)
{
    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new CardsVar(2),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + bonus);

        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);

        await PowerCmd.Apply<IntangiblePower>(Owner.Creature, 1m, Owner.Creature, this);

        var arms = DynamicVars.Cards.IntValue + (tier >= 200 ? 1 : 0);
        for (var i = 0; i < arms; i++)
        {
            var card = Owner.Creature.CombatState.CreateCard<KnightsArm>(Owner);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 0.8f);
        }
    }
}
