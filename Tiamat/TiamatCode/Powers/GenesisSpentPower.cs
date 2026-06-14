using MegaCrit.Sts2.Core.Entities.Powers;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Marker: la ventana de NP "Génesis" ya se abrió este pico (la carga llegó a 100). Se
/// re-arma al bajar el medidor por debajo de 100 (ver MainFile).
/// </summary>
public sealed class GenesisSpentPower : TiamatPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
