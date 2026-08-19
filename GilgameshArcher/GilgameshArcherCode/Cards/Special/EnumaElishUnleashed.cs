using GilgameshArcher.GilgameshArcherCode.Cards;
using GilgameshArcher.GilgameshArcherCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// ENUMA ELISH: Desatado / 天地乖离开辟之星·解放 (DESIGN-GILGAMESH §6) — la ulti AUTO-MANIFESTADA.
/// Cuando la Carga NP cruza 100, <see cref="MainFile"/> (vía <c>NpCharge.GaugeFilled</c>) genera ESTA
/// carta a tu mano: 0⚡, Retain, Exhaust. Consume TODA la carga (mín. 100, <c>ConsumeAllForNpCard</c>);
/// al gastarla por debajo de 100 el marcador se re-arma para el próximo pico.
///
/// FIDELIDAD EXACTA al NP real («el daño base NO escala con OC; el multiplicador superefectivo SÍ»):
/// - 30 de daño a TODOS los enemigos (plano contra «lo meramente humano» — salas comunes).
/// - Contra Élites y Jefes (<see cref="RoyalTrait.IsDivine"/>, lo de rango divino del Spire): +15 base,
///   y ese bonus anti-divino sube +4 por cada 20 de carga consumida sobre 100 (la Sobrecarga). A
///   consumo 300 (cap): 30 / 85 vs Élite-Jefe (overcharge = (300-100)/20*4 = 40; divino = 30 + 15 + 40 =
///   85, antes de NpLevels) — calca la curva OC100→500 (×1.5→×2.0) usando el cap 300 de FGOCore como
///   OC500. La decisión de ATESORAR hasta 300 ES el Overcharge.
/// - El daño base escala SOLO con dupes (<see cref="NpLevels.Scale"/>, +15%/nivel = NP1→NP5 300%→500%).
/// - La <c>OverchargeBlessingPower</c> (El Que Vio el Abismo / Decreto del Rey) ya está horneada en
///   <c>ConsumeAllForNpCard</c>: sube el tier consumido antes de calcular el bonus anti-jefe.
/// </summary>
public sealed class EnumaElishUnleashed() : GilgameshCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IGilgameshNpCard, ICommandTyped
{
    public const int ChargeCost = 100;

    // TAREA D: tipo de NP del juego original (Enuma Elish = Buster) → bonus de ulti del sistema de tipos.
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => true;

    private const int PerTwenty = 4;     // +4 al bonus anti-divino por cada 20 de carga sobre 100
    private const int PerArmInHand = 1;  // +1 al bonus anti-divino por cada Arma del Tesoro exhaustada (P2)

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(30m, ValueProp.Move),
        new DynamicVar("Divine", 15),       // bonus base vs Élite/Jefe
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<OverchargeBlessingPower>()
    ];

    // El waiver NO cubre Event (parche P3): sin pasar `this`, CanPay daría glow/playable falsos con el
    // medidor vacío y un waiver activo en mano.
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1) Consume TODA la carga; tier = lo realmente consumido (>= 100, + OverchargeBlessing).
        var tier = await NpCharge.ConsumeAllForNpCard(choiceContext, Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 20 * PerTwenty; // SÓLO escala el bonus anti-divino

        // 1.bis) FIX HOMOGENEIZACIÓN (P2): las Armas del Tesoro que queden en la mano se EXHAUSTAN como
        //   Sobrecarga extra de la PROPIA tribu de Gil (+1 al bonus anti-divino c/u) — el gasto del NP
        //   depende de su arsenal, no solo de la carga genérica del medidor. Fiel al OC: SÓLO escala el
        //   bonus anti-divino, nunca el daño base contra «lo meramente humano».
        // Solo en salas con rango divino (audit 2026-07-05): el bonus por Arma aplica unicamente a
        // Elites/Jefes — exhaustar el arsenal en una sala comun era puro costo sin efecto.
        //
        // ORDEN (reporte de Steam 2026-08-18, Nut Butter: «el NP se traba en el medio de la pantalla y
        // no hace nada»): acá SOLO se LEE el arsenal; el Agotar corre al FINAL, después del daño. Una
        // carta jugada vive en el PlayContainer —el centro exacto de la pantalla— hasta que su OnPlay
        // retorna, así que cualquier await que no vuelva la deja congelada ahí. `CardCmd.Exhaust` con
        // visuales entra a `CardPileCmd.Add`, que espera un tween (`await tween.AwaitFinished`) DENTRO
        // de nuestra propia resolución; con el daño primero, aunque esa espera falle el NP ya pegó.
        var arms = new List<CardModel>();
        if (Owner.PlayerCombatState != null && RoyalTrait.IsInDivineRoom(Owner.Creature))
        {
            arms.AddRange(Owner.PlayerCombatState.Hand.Cards.OfType<ITreasureArm>().Cast<CardModel>());
        }
        overcharge += arms.Count * PerArmInHand;

        // 2) El bonus anti-divino se calcula POR objetivo: a cada enemigo según sea o no de rango divino.
        //    El daño base (30) es plano para todos; sólo Élites/Jefes reciben +Divine (+overcharge).
        //    La animación de ataque se dispara UNA sola vez para todo el NP (patrón OnlyPlayAnimOnce de
        //    Astolfo): un Execute por enemigo la repetía N veces, con su espera por cada repetición.
        var animPlayed = false;
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            var divineBonus = RoyalTrait.IsDivine(enemy) ? DynamicVars["Divine"].IntValue + overcharge : 0;
            var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + divineBonus);
            var attack = DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(enemy)
                .WithHitFx("vfx/vfx_starry_impact");
            if (animPlayed) attack = attack.WithNoAttackerAnim();
            animPlayed = true;
            await attack.Execute(choiceContext);
        }

        // 3) Recién ahora se cobra el arsenal, y sin visuales (patrón ShutenDouji.ExhaustSiblingNp):
        //    el bonus ya está horneado en el daño de arriba, así que este paso no puede robárselo.
        foreach (var arm in arms)
        {
            await CardCmd.Exhaust(choiceContext, arm, skipVisuals: true);
        }
    }

    // No drafteable (se manifiesta sola), pero definimos el up por consistencia con el ecosistema:
    // sube SÓLO la base 30→34 (perilla §10 «si Enuma se siente débil en salas comunes, subir la base
    // ANTES de tocar el bonus anti-jefe — preservar la fidelidad del OC»).
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
