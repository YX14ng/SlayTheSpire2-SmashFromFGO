using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Cards;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// «Rye Rhyme Goodfellow» (彼方にかざす夢の噺 / 向彼方高举的梦之话) — DESIGN-OBERON §6.4, la carta-NP
/// drafteable. 2⚡ (mín 70, consume TODA la carga) · Exhaust · Ataque NP: 22 de daño a TODOS; removés la
/// Fuerza positiva de todos; todos se DUERMEN. SOBRECARGA: +<see cref="PerTen"/> por cada 10 sobre el
/// mínimo. +15%/nivel (NpLevels). Glow al gate. up +6 daño.
/// </summary>
public sealed class RyeRhymeGoodfellow() : OberonCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IOberonNpCard
{
    public const int ChargeCost = 70;
    private const int PerTen = 3;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(22m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", PerTen)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge);

        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);

        await OberonExtensions.StripPositiveStrengthFromAll(Owner.Creature.CombatState, Owner.Creature);
        await OberonExtensions.SleepAll(Owner.Creature.CombatState, Owner.Creature, Powers.Sleep.Sleep.DefaultDuration, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}
