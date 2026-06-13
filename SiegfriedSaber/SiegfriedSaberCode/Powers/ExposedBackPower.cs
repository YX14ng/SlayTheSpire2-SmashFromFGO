using FGOCore.FGOCoreCode.DragonScales;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Espalda Expuesta (露背 / Exposed Back) — el flag de "Sangre de Dragón SUPRIMIDA este turno"
/// que aplica la carta homónima (DESIGN-SIEGFRIED §7). Mientras esté presente, las escamas no
/// reducen NINGÚN golpe entrante (el espejo riesgoso: baja la guardia para golpear más fuerte).
///
/// Implementa <see cref="ISdDSuppressor"/>, que <see cref="DragonScalesPower"/> consulta como
/// LECTURA PURA por cada golpe (a prueba de previews de daño). El flag NO se muta por golpe: es
/// un power Single que se auto-remueve al terminar el turno del jugador (AfterSideTurnEnd cuando
/// el owner participó del turno que acaba) — patrón de CustomTemporaryPowerModel.
///
/// Interacción anti-degeneración: la supresión NO alimenta el refund de NP de la Hoja de Tilo.
/// Un golpe no reducido es "alcance pleno" (como uno traspasado), y la regla P2 de la reliquia
/// sólo paga NP en golpes que las escamas SÍ encogieron a residual ≥1. No hay batería extra por
/// jugar esta carta — sólo el riesgo de comer el daño completo.
/// </summary>
public sealed class ExposedBackPower : SiegfriedPower, ISdDSuppressor
{
    public override PowerType Type => PowerType.Debuff; // es un riesgo autoinfligido (baja tu defensa)

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public bool SuppressScales => true;

    // Patrón FlameBarrierPower (vanilla): aplicado en tu turno, cubre la fase de ataque enemiga
    // entrante (el riesgo) y se remueve al terminar el turno enemigo (Owner.Side != side), justo
    // antes de que arranque tu próximo turno. "Este turno" en términos de StS para un buff
    // defensivo = la ventana de golpes entrantes que sigue a jugarlo.
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
