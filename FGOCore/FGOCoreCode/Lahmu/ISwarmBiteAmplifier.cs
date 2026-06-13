namespace FGOCore.FGOCoreCode.Lahmu;

/// <summary>
/// Suma mordidas EXTRA del enjambre por turno. La forma Bestia de Tiamat lo implementa con
/// <c>ExtraBites => 1</c> para que el enjambre "muerda dos veces". <see cref="LahmuSwarmPower"/>
/// suma el <see cref="ExtraBites"/> de cada power del Owner. (Gancho en FGOCore para no
/// referenciar el mod Tiamat desde la librería compartida; espejo de ICurseAmplifier.)
/// </summary>
public interface ISwarmBiteAmplifier
{
    int ExtraBites { get; }
}
