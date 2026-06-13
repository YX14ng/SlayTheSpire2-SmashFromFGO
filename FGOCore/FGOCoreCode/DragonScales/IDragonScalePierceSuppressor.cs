namespace FGOCore.FGOCoreCode.DragonScales;

/// <summary>
/// OPUESTO a <see cref="ISdDSuppressor"/>: cubre la espalda expuesta. Mientras un power con
/// <see cref="SuppressPierce"/> esté activo, el piercer (la Hoja de Tilo) NO deja pasar el primer golpe que
/// te alcanza — las escamas lo reducen como a cualquier otro — y se cortan TODAS las vías de NP que ese
/// pierce/golpe-que-alcanza alimentaría (el +NP por golpe reducido y los listeners del pierce). Lo consulta
/// el piercer (la reliquia), NO DragonScalesPower. Lectura PURA, a prueba de previews — NUNCA mutar estado.
/// </summary>
public interface IDragonScalePierceSuppressor
{
    bool SuppressPierce { get; }
}
