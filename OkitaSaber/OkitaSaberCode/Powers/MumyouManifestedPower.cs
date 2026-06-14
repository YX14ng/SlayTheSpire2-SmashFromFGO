using MegaCrit.Sts2.Core.Entities.Powers;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Marca (oculta) de que el «Mumyou Sandanzuki: Desatado» ya se manifestó en este pico de 100 NP
/// (patrón CamelotManifestedPower de Mash). Se aplica al cruzar 100 (NpCharge.GaugeFilled) y se
/// remueve al bajar < 100 (NpCharge.GaugeDropped), re-armando la manifestación del próximo pico.
/// Single, invisible.
/// </summary>
public sealed class MumyouManifestedPower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;
}
