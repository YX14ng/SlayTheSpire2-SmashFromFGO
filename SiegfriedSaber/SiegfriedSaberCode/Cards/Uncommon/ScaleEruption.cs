using FGOCore.FGOCoreCode.DragonScales;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Erupción de Escamas (鳞片爆发 / Scale Eruption) — el SINK ACTIVO de la Sangre de Dragón
/// (mejora de revisión P1). Hasta hoy la SdD sólo SUBÍA y casi nunca se gastaba por elección
/// (sólo el pierce pasivo de la Hoja de Tilo la "consumía"). Esta carta convierte la divisa
/// defensiva en daño: descargás toda la armadura como una tormenta de escamas.
///
/// 1⚡ Ataque AoE: consumí TODA tu Sangre de Dragón; inflige <see cref="DamagePerPoint"/> (3) de
/// daño a TODOS por punto consumido, hasta <see cref="DamageCap"/> (6) puntos (máx 18 AoE; up 24).
/// Sin daño base plano: es PURO payoff del recurso de firma (la decisión central que faltaba —
/// ¿mantengo la armadura gruesa para tanquear y afilar a Balmung, o la descargo AHORA como burst,
/// quedando expuesto?). Patrón consume-recurso→AoE-por-punto idéntico a LieLikeVortigern de Oberon.
///
/// BALANCE (SKILL §2/§3, techo §1.bis 180-220/turno):
/// - 1⚡ Uncommon AoE puro = ~10-12 base; aquí el daño NO es plano: arranca en 0 y SÓLO sube si
///   invertiste turnos en construir SdD (Bautismo de Sangre, Hoja de Tilo, etc.) — el escalado se
///   GANA, una condición que puede fallar de verdad (§3). Con SdD ≥6 = 18 AoE, dentro del rango.
/// - El cap 6 al DAÑO (no a lo consumido) es el candado anti-techo: por encima de 6 de SdD la
///   armadura se gasta igual pero el daño no crece (no se acumula un nuke ilimitado, lección
///   TyrantsSweep P3). Multi-objetivo: 18×N enemigos sigue bajo el techo a 2-3 enemigos.
/// - El COSTE real es quedarte sin escamas: pierdes la reducción por-golpe Y desafilas a Balmung
///   (overcharge cae a +1/10 si SdD &lt;3). Riesgo-recompensa temático genuino, no un motor gratis.
/// - up: +1 por punto (DamagePerPoint 3→4, máx 24). No sube el cap (preserva el techo §1.bis).
/// </summary>
public sealed class ScaleEruption() : SiegfriedCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    private const int DamagePerPoint = 3; // daño AoE por punto de SdD consumido (up 4)
    private const int DamageCap = 6;      // tope de puntos que cuentan para el daño (candado de techo)

    // DamageVar base 0 (el daño REAL se calcula en OnPlay desde la SdD consumida): toda carta Attack del
    // pool declara un DamageVar para clasificarse/renderizarse como ataque. La loc no usa !D! (sólo
    // !PerPoint!), así que el 0 nunca aparece en el texto — el daño mostrado es la erupción calculada.
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(0m, ValueProp.Move),
        new DynamicVar("PerPoint", DamagePerPoint)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    // Glow cuando hay armadura que descargar (lectura pura del owner, preview-safe).
    protected override bool ShouldGlowGoldInternal => Scales > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Consumí TODA la SdD (descargás la armadura entera); el DAÑO se capa a 6 puntos.
        var scales = Scales;
        if (scales > 0)
        {
            var power = Owner.Creature.GetPower<DragonScalesPower>();
            if (power != null) await PowerCmd.Remove(power);
        }

        var counted = System.Math.Min(scales, DamageCap);
        var damage = counted * DynamicVars["PerPoint"].IntValue;
        if (damage <= 0) return; // sin armadura, sin erupción (carta muerta — es el riesgo de jugarla en seco)

        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars["PerPoint"].UpgradeValueBy(1m);
}
