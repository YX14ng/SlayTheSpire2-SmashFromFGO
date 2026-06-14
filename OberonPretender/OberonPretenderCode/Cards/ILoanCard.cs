namespace OberonPretender.OberonPretenderCode.Cards;

/// <summary>
/// Marcador de carta de PRESTAMO (Loan) -- la tribu de Oberon: toda carta que te da recurso AHORA y
/// suma Deuda al jugarse. Patron marcador (IDragonSlayerCard/ISiegfriedNpCard): la "keyword" PRESTAMO
/// vive en el nombre/lore y en el tag visible; el codigo la lee como <c>card is ILoanCard</c>.
///
/// Los lectores del kit usan este marcador SIN tocar FGOCore: el limite de credito (la base
/// <see cref="OberonCard"/> apaga su glow/jugabilidad a Deuda >= <see cref="Powers.DebtPower.CreditLimit"/>),
/// la pasiva del Rey del Cuento (primer prestamo del turno = endulzado), la Corona de Flores del
/// Principe (el primer prestamo del combate no genera Deuda), el poder Interes a Mi Favor y el
/// Monedero del Prestamista. Implementarlo NO aplica Deuda solo: cada carta llama a
/// <see cref="Powers.DebtPower"/> en su OnPlay con la denominacion de su prestamo (1/2/4).
/// </summary>
public interface ILoanCard;
