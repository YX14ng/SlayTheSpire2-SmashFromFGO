using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Cards;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Cards.Special;

/// <summary>
/// «Rye Rhyme Goodfellow: Desatado» (彼方にかざす夢の噺 / 向彼方高举的梦之话) — la Desatada que se
/// auto-manifiesta a 100 NP en El Rey del Cuento o El Príncipe del Invierno (handler GaugeFilled).
/// Ataque NP 0⚡ Exhaust (DESIGN-OBERON §6.5):
///   30 de daño a TODOS; removés la Fuerza positiva de todos; consume TODA la carga.
///   SOBRECARGA: +<see cref="PerTen"/> por cada 10 sobre 100. A sobrecarga ≥<see cref="MassSleepTier"/>
///   (150): todos los enemigos se DUERMEN (el sueño masivo vive en la sobrecarga, P1 Morgan).
/// +15%/nivel (NpLevels). Event → ningún waiver la cubre (P3); consume el medidor entero.
/// </summary>
public sealed class RyeRhymeGoodfellowUnleashed() : OberonCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IOberonNpCard
{
    public const int ChargeCost = 100;
    private const int PerTen = 3;
    private const int MassSleepTier = 150;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(30m, ValueProp.Move),
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

        // El sueño masivo vive en la sobrecarga (≥150): paga por FRECUENCIA, no por daño base.
        if (tier >= MassSleepTier)
        {
            await OberonExtensions.SleepAll(Owner.Creature.CombatState, Owner.Creature, Powers.Sleep.Sleep.DefaultDuration, this);
        }
    }
}
