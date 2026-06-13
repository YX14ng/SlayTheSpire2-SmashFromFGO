using FGOCore.FGOCoreCode.DragonScales;
using FGOCore.FGOCoreCode.Np;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Basic;

/// <summary>
/// Tajo Cazadragones (屠龙之击) — firma básica BUSTER con sinergia (DESIGN-SIEGFRIED §5: "otra Buster
/// o de bloqueo-con-sinergia"). Es el Buster de identidad: pega más MIENTRAS la armadura es gruesa
/// (SdD ≥ 3) — la MISMA lente que el overcharge de Balmung ("la sangre del dragón afila el filo", él
/// ES el dragón). Carga un poco de NP al golpear (enseña que los Buster también llenan el medidor).
///
/// El bonus de SdD adelanta en miniatura la keyword Cazadragones del pool (§10, aún sin implementar
/// como CardKeyword): acá la sinergia se lee directo del contador de Sangre de Dragón, sin depender
/// de un hook nuevo todavía.
///
/// Balance (SKILL §2): 1⚡ Común ataque puro = 9-10. Base 9. El +3 (→12) está GATEADO por SdD ≥ 3,
/// una condición de estado que PUEDE fallar de verdad: arrancás en SdD 2, hay que cruzar el umbral
/// (un Bautismo de Sangre, procs de la Hoja de Tilo). §3: condición de estado paga +20-30% → 9×1.3
/// ≈ 12. El +6 NP es rider de medio-⚡ recortado (SKILL §2 "~10 NP ≈ ½⚡" → 6 ≈ 0.3⚡), pagado bajando
/// la base de 10 a 9. Techo: 12/1⚡ &lt; el ×1.5 de motor / ×1.25 de tasa cruda (SKILL §1.bis: 10×1.25
/// = 12.5). No es loop AFK: el +NP exige JUGAR la carta y conectar el ataque.
/// </summary>
public sealed class DragonbloodCut() : SiegfriedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IDragonSlayerCard
{
    private const int ScalesThreshold = 3;  // mismo umbral que afila a Balmung (§4)
    private const int NpGain = 6;           // SKILL §2: ~10 NP ≈ ½⚡; 6 ≈ 0.3⚡ recortado de la base

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Bonus", 3),
        new DynamicVar("Np", NpGain)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DragonScalesPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    // Resplandor cuando la armadura afila el filo (espejo del glow de Balmung con el mismo umbral).
    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.GetPowerAmount<DragonScalesPower>() >= ScalesThreshold;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = Owner.Creature.GetPowerAmount<DragonScalesPower>() >= ScalesThreshold
            ? DynamicVars["Bonus"].IntValue
            : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    // Up: +3 a la base (9→12; con SdD≥3 llega a 15 = pico de Común poco-común, pagado por la
    // condición + mejora). No subimos el bonus condicional para no inflar el techo gateado.
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
