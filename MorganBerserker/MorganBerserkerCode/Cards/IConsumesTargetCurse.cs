namespace MorganBerserker.MorganBerserkerCode.Cards;

/// <summary>
/// Marcador: Ataque que consume la Maldición DEL OBJETIVO como parte de su PROPIO efecto
/// (p.ej. <see cref="Rare.FinalCollection"/>: consume toda la Maldición y la convierte en daño 2×).
/// La pasiva "Sentencia" de las formas Reina/Invierno consume la Maldición del objetivo en
/// <c>BeforeCardPlayed</c> — que corre ANTES del <c>OnPlay</c> de la carta — así que, sin exención,
/// le roba la Maldición a estos Ataques: su <c>Curses.Consume</c> devuelve 0 → la carta sale temprano
/// y NO hace daño (reporte de player: "金卡消耗诅咒打伤害，伤害没出来"). Las formas saltean la
/// Sentencia para las cartas con este marcador y les dejan la Maldición intacta.
/// </summary>
public interface IConsumesTargetCurse;
