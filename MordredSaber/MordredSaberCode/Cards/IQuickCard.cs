namespace MordredSaber.MordredSaberCode.Cards;

/// <summary>
/// Marcador de carta de comando *Quick* de Mordred (la verde: daño bajo + Estrellas de
/// Crítico). La implementa <c>QuickMordred</c> (fase Content). Lo lee la reliquia de tienda
/// «Moto Roja de Trifas» (Riding B: +10★ a tus cartas Quick). Patrón ISiegfriedNpCard /
/// IMordredNpCard — un marcador de sub-familia, sin lógica, para que las reliquias citen al
/// arquetipo de comando sin acoplarse a un id concreto.
/// </summary>
public interface IQuickCard;
