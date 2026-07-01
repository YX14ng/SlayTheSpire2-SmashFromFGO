namespace MorganBerserker.MorganBerserkerCode.Cards;

/// <summary>
/// Marcador: Ataque que USA la Maldición DEL OBJETIVO como parte de su propio efecto — ya sea
/// CONSUMIÉNDOLA (p.ej. <see cref="Rare.FinalCollection"/>: toda la Maldición → daño 2×) o
/// LEYÉNDOLA para un rider condicional (p.ej. <see cref="Uncommon.BarghestsFang"/> cura si el objetivo
/// tiene Maldición; <see cref="Common.QueensScorn"/> +daño; <see cref="Common.RoyalPunishment"/> +NP).
///
/// La pasiva "Sentencia" de las formas Reina/Invierno consume la Maldición del objetivo en
/// <c>BeforeCardPlayed</c> — que corre ANTES del <c>OnPlay</c> de la carta. Sin exención, le roba la
/// Maldición a estos Ataques antes de que ellos la usen: la consumidora ve 0 y no hace daño (reporte:
/// "金卡消耗诅咒打伤害，伤害没出来"); las lectoras ven <c>Curses.Of(target)==0</c> y su rider NO dispara
/// (reporte: "Barghest's Fang says it heals if the target has curse but it doesn't heal"). Las formas
/// SALTEAN la Sentencia para las cartas con este marcador y les dejan la Maldición intacta, así cada
/// carta la maneja ella misma (evita también el doble-dip Sentencia + rider).
/// </summary>
public interface IUsesTargetCurse;
