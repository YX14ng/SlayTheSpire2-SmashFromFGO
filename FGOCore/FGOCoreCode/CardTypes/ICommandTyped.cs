namespace FGOCore.FGOCoreCode.CardTypes;

/// <summary>
/// La implementa toda carta con tipo de comando FGO (Buster/Arts/Quick) para recibir el bonus
/// automático de <see cref="CommandBonusPower"/> al jugarse. Las cartas de COMANDO básicas la
/// implementan con <see cref="IsNoblePhantasm"/> = false; las cartas de ULTI (Noble Phantasm)
/// con <see cref="IsNoblePhantasm"/> = true para recibir el bonus reforzado (ulti) en vez del normal.
///
/// El bonus NO requiere que la carta haga nada en su <c>OnPlay</c>: el hook central
/// (<see cref="CommandBonusPower.AfterCardPlayed"/>, sembrado en TODOS los Servants por
/// <see cref="Bond.BondRelic"/>) lee este tipo después de resolver la carta y aplica el efecto.
/// </summary>
public interface ICommandTyped
{
    /// <summary>El color del comando (Buster/Arts/Quick).</summary>
    CommandType CommandType { get; }

    /// <summary>True si esta carta es un Noble Phantasm (ulti): activa el bonus reforzado.</summary>
    bool IsNoblePhantasm { get; }
}
