using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>
/// Espalda Expuesta (露背一击 / Exposed Back) — DESIGN-SIEGFRIED §7. 1⚡ Ataque: 14 de daño
/// (up 18); este turno tu Sangre de Dragón NO reduce ningún golpe (el espejo riesgoso de su
/// debilidad canónica: deja la espalda al aire para descargar un tajo más limpio). Aplica el
/// power Single <see cref="ExposedBackPower"/> (ISdDSuppressor) que DragonScalesPower respeta.
///
/// Anti-degeneración (P2 ya cubierta relic-side): la supresión NO genera +NP — un golpe no
/// reducido es "alcance pleno", y la Hoja de Tilo sólo refunda en golpes que las escamas SÍ
/// encogieron. No hay batería gratis por jugar esta carta; sólo el riesgo de comer todo el daño.
///
/// BALANCE (SKILL §2/§3, 1⚡ Común Ataque con downside autoinfligido):
/// - §2 "Común / 1⚡ puro = 9-10". El piso de un 1⚡ común puro es 9-10.
/// - El downside (tu SdD reduce 0 este turno = comés el daño completo) es un AUTO-DAÑO
///   variable. §3 "Costo de HP (N de vida) ≈ +2N de efecto" + la analogía vanilla Hemokinesis
///   (15 a 1⚡ por 2 HP fijos). Acá el riesgo NO es fijo (un turno con pocos golpes entrantes
///   cuesta poco; uno con un jefe pegando fuerte cuesta mucho), así que se cobra MENOS que el
///   2-HP fijo de Hemokinesis → base 14 (no 15): 10 puro + ~+40% por el downside esquivable.
/// - 14 cae dentro del techo §1.bis (tasa cruda ×1.25 sobre el 10 puro = 12.5; el +1.5 extra lo
///   paga el downside genuino, no un motor). No es un nuke gateado: es el espejo de su identidad.
/// - Up 14→18 (+4): §2 los upgrades de ataque rondan +3/+4 (Strike +3 acá). 18 a 1⚡ con downside
///   refleja el "18 con downside" que §2 lista a 2⚡ común, comprimido a 1⚡ porque el castigo
///   (perder toda la SdD un turno) es más severo que el genérico "−2 HP" → sigue honesto.
/// </summary>
public sealed class ExposedBack() : SiegfriedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Baja la guardia ESTE turno (las escamas reducen 0); el flag se va solo al fin del turno.
        await PowerCmd.Apply<ExposedBackPower>(Owner.Creature, 1, Owner.Creature, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
