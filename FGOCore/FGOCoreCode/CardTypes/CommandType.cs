namespace FGOCore.FGOCoreCode.CardTypes;

/// <summary>
/// Tipo de carta de comando FGO (la "color" del comando). En FGO cada carta de comando y cada
/// Noble Phantasm tiene uno de estos tres tipos, cada uno con un payoff distinto:
///   Buster (roja)  → golpe pesado / daño.   Bonus FGOCore: +Fuerza (temporal normal, permanente si NP).
///   Arts   (azul)  → carga de medidor.       Bonus FGOCore: +Carga NP (solo el NP — las Arts normales ya cargan por su propio efecto).
///   Quick  (verde) → generación de Estrellas. Bonus FGOCore: +Estrellas de Crítico.
/// El bonus lo aplica <see cref="CommandBonusPower"/> al jugar cualquier carta que implemente
/// <see cref="ICommandTyped"/>. Montos por defecto y reglas: ver <see cref="CommandBonusPower"/>.
/// </summary>
public enum CommandType
{
    Buster,
    Arts,
    Quick
}
