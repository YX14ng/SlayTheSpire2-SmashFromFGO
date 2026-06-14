namespace OberonPretender.OberonPretenderCode.Cards;

/// <summary>
/// Marcador de carta de comando *Quick* de Oberon (la verde: daño bajo + Estrellas de Critico). La
/// implementara la carta Quick del pool de comando (fase Content / FGOCore Cards/Command pendiente).
/// Lo lee la reliquia «Polilla Halcon» (Riding A: la primera Quick del turno roba 1). Patron
/// IQuickCard de Mordred / ILoanCard — un marcador de sub-familia, sin logica, para que las reliquias
/// citen al arquetipo de comando sin acoplarse a un id concreto.
/// </summary>
public interface IQuickCard;
