namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Cabalgar E (骑乘E) — al inicio de cada turno: +<see cref="Amount"/> *Estrellas de Crítico
/// (DESIGN-OKITA §5.3, "sabe montar… apenas": +5; up +10). El goteo mínimo del rango E.
/// Counter (guarda las estrellas/turno). Personal: no escala en multijugador.
/// </summary>
public sealed class RidingEPower : PerTurnStarsPower;
