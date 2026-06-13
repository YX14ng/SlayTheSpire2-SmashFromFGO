namespace FGOCore.FGOCoreCode.Lahmu;

/// <summary>
/// Marca (tag) que amplifica el daño de Devorar. La forma Bestia de Tiamat lo implementa para
/// "Devorar +50%". Las cartas que devoran multiplican su daño por
/// <see cref="Lahmu.DevourBonusMultiplierPct"/> (150 si la dueña tiene un IDevourAmplifier).
/// </summary>
public interface IDevourAmplifier
{
}
