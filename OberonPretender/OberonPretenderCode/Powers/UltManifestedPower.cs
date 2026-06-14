using MegaCrit.Sts2.Core.Entities.Powers;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Marcador interno: la Desatada de Oberon ya se manifesto en este pico de medidor (>=100). Evita que
/// el handler de <c>NpCharge.GaugeFilled</c> genere una segunda carta-ulti mientras el medidor sigue
/// >=100. Se remueve al caer < 100 (<c>GaugeDropped</c>), re-armando la proxima manifestacion.
/// Patron CamelotManifestedPower/NpManifestedPower.
/// </summary>
public sealed class UltManifestedPower : OberonPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
