using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.CardTypes;
using OberonPretender.OberonPretenderCode.Cards;
using OberonPretender.OberonPretenderCode.Extensions;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Special;

/// <summary>
/// «Rye Rhyme Goodfellow: Desatado» (彼方にかざす夢の噺 / 向彼方高举的梦之话) — la Desatada que se
/// auto-manifiesta a 100 NP en El Rey del Cuento o El Príncipe del Invierno (handler GaugeFilled).
/// Ataque NP 0⚡ Exhaust (DESIGN-OBERON §6.5):
///   30 de daño a TODOS; removés la Fuerza positiva de todos; consume TODA la carga.
///   SOBRECARGA: +<see cref="PerTen"/> por cada 10 sobre 100. A sobrecarga ≥<see cref="MassSleepTier"/>
///   (150): todos los enemigos se DUERMEN (el sueño masivo vive en la sobrecarga, P1 Morgan).
/// +15%/nivel (NpLevels). Event → ningún waiver la cubre (P3); consume el medidor entero.
///
/// REDISEÑO P1 (deshomogeneizar el NP — mejora de revisión): el ulti deja de ser genérico y pasa a
/// depender del RECURSO DE FIRMA. Tras consumir la carga, consume hasta <see cref="DebtConsumeCap"/> (5)
/// puntos de Deuda y suma <see cref="DamagePerDebt"/> (2) de daño AoE por punto consumido — el sueño que
/// el prestamista vendió se cobra contra el mundo (reusa la lógica VortigernPower/LieLikeVortigern de
/// consumir Deuda → daño). El daño base ya NO es independiente de lo que el mazo construyó: cuanta más
/// Deuda diferiste, más golpea la Desatada (y te la saca de encima antes del cobro de fin de turno).
/// </summary>
public sealed class RyeRhymeGoodfellowUnleashed() : OberonCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IOberonNpCard, ICommandTyped
{
    public const int ChargeCost = 100;

    // TAREA D: tipo de NP del juego original (Rye Rhyme Goodfellow = Buster) → bonus de ulti del sistema de tipos.
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => true;
    private const int PerTen = 3;
    private const int MassSleepTier = 150;
    private const int DamagePerDebt = 2;     // +2 daño AoE por punto de Deuda consumido (P1)
    private const int DebtConsumeCap = 5;    // tope: hasta 5 puntos (espeja DebtPower.VortigernUnpaidCap)

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(30m, ValueProp.Move),
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
        var tier = await NpCharge.ConsumeAllForNpCard(choiceContext, Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        // P1: consume hasta 5 puntos de Deuda → +2 daño AoE por punto (el recurso de firma alimenta el
        // ulti; patrón LieLikeVortigern: Forgive devuelve lo realmente consumido).
        var consumable = Math.Min(DebtPower.Of(Owner.Creature), DebtConsumeCap);
        var consumed = await DebtPower.Forgive(choiceContext, Owner.Creature, consumable);
        var debtBonus = consumed * DynamicVars["PerDebt"].IntValue;

        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge + debtBonus);

        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);

        await OberonExtensions.StripPositiveStrengthFromAll(choiceContext, Owner.Creature.CombatState!, Owner.Creature);

        // El sueño masivo vive en la sobrecarga (≥150): paga por FRECUENCIA, no por daño base.
        if (tier >= MassSleepTier)
        {
            await OberonExtensions.SleepAll(choiceContext, Owner.Creature.CombatState!, Owner.Creature, Powers.Sleep.Sleep.DefaultDuration, this);
        }
    }
}
