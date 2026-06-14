using MegaCrit.Sts2.Core.Entities.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Marcador OCULTO "Enuma Elish ya se manifestó este pico" (DESIGN-GILGAMESH §6, patrón
/// <c>CamelotManifestedPower</c> de Mash). Cruzar 100 de Carga NP manifiesta la carta-ulti UNA
/// vez por pico; este flag evita re-manifestarla a cada ganancia mientras seguís sobre 100.
/// <see cref="MainFile"/> lo aplica en <c>NpCharge.GaugeFilled</c> y lo retira en
/// <c>NpCharge.GaugeDropped</c> (al caer bajo 100 se re-arma para el próximo pico).
///
/// Es infraestructura, no un buff visible: silencioso, sin escalado MP, sin icono propio
/// (cae al placeholder de la base si llegara a mostrarse).
/// </summary>
public sealed class EnumaManifestedPower : GilgameshPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldScaleInMultiplayer => false;
}
