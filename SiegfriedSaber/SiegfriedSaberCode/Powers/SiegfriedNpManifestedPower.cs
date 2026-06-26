using MegaCrit.Sts2.Core.Entities.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Marca (oculta) de que la carta-NP «Balmung: Desatado» ya se manifestó en este pico de 100 NP
/// (patrón MumyouManifestedPower de Okita / NpManifestedPower de Morgan). Se aplica al cruzar 100
/// (NpCharge.GaugeFilled) y se remueve al bajar &lt; 100 (NpCharge.GaugeDropped), re-armando la
/// manifestación del próximo pico. Single, invisible.
/// </summary>
public sealed class SiegfriedNpManifestedPower : SiegfriedPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;
}
