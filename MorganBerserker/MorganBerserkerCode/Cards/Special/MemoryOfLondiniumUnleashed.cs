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
/// AoE + Knight's Arms, free, Exhaust. At 200+ consumed: Intangible 1 y +1 Arm.
/// Playtest 2026-06-11: subida sobre Roadless (40 vs 36, overcharge 4/10 vs 3/10,
/// 3 Brazos) — la Bruja es la forma lenta de carga, su payoff pega más fuerte.
/// Parche del juez P1 (rediseño v2): el Intangible se mueve al overcharge (tier
/// >= 200) — con la economía nueva, a 100 planos era WraithForm gratis cada turno;
/// gateado restaura «sobrecargar = decisión real». Daño 40/PerTen 4 + 3 Armas quedan.
/// </summary>
public sealed class MemoryOfLondiniumUnleashed() : MorganCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies)
{
    public const int ChargeCost = 100;

    /// <summary>Tier de sobrecarga: con esta carga consumida o más, también da Intangible y +1 Arma.</summary>
    public const int OverchargeTier = 200;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(40m, ValueProp.Move),
        new CardsVar(3),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 4)
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

        if (tier >= OverchargeTier)
        {
            await PowerCmd.Apply<IntangiblePower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        var arms = DynamicVars.Cards.IntValue + (tier >= OverchargeTier ? 1 : 0);
        for (var i = 0; i < arms; i++)
        {
            var card = Owner.Creature.CombatState.CreateCard<KnightsArm>(Owner);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 0.8f);
        }
    }
}
