using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Cards;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Special;

/// <summary>
/// «Lie Like Vortigern: Desatado» (谎言如沃提庚) — la Desatada que se auto-manifiesta a 100 NP en
/// VORTIGERN (handler GaugeFilled). Ataque NP 0⚡ Exhaust (DESIGN-OBERON §6.5):
///   35 de daño a TODOS; consume TODA tu Deuda: +<see cref="DamagePerDebt"/> de daño por punto
///   consumido (ignora Dormido sin despertar — VortigernPower es ISleepIgnorer). SOBRECARGA:
///   +<see cref="PerTen"/> por cada 10 sobre 100. Sin sueño — el dragón no arrulla, devora.
/// +15%/nivel (NpLevels). El +3 de Ataque de Vortigern se suma vía ModifyDamageAdditive.
/// </summary>
public sealed class LieLikeVortigernUnleashed() : OberonCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IOberonNpCard
{
    public const int ChargeCost = 100;
    private const int PerTen = 4;
    private const int DamagePerDebt = 3;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(35m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", PerTen),
        new DynamicVar("PerDebt", DamagePerDebt)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<DebtPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        // Consume TODA la Deuda → +3 daño por punto (el default declarado contra el mundo).
        var debt = DebtPower.Of(Owner.Creature);
        var consumed = await DebtPower.Forgive(Owner.Creature, debt);
        var debtBonus = consumed * DynamicVars["PerDebt"].IntValue;

        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge + debtBonus);
        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }
}
