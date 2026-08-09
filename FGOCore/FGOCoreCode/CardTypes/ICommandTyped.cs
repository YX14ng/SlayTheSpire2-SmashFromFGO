using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;

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
public interface ICommandTyped : ICustomTypeTextCard
{
    /// <summary>El color del comando (Buster/Arts/Quick).</summary>
    CommandType CommandType { get; }

    /// <summary>True si esta carta es un Noble Phantasm (ulti): activa el bonus reforzado.</summary>
    bool IsNoblePhantasm { get; }

    /// <summary>
    /// BaseLib 3.4 muestra el tipo de comando junto al tipo normal de la carta
    /// (por ejemplo, "Buster Attack") sin duplicarlo en cada descripción.
    /// </summary>
    IEnumerable<LocString> ICustomTypeTextCard.GetTypeModifiers() => [CommandTypeText.Get(CommandType)];
}

internal static class CommandTypeText
{
    internal static LocString Get(CommandType type)
    {
        var key = type switch
        {
            CommandType.Buster => "FGOCORE-COMMAND_TYPE_BUSTER",
            CommandType.Arts => "FGOCORE-COMMAND_TYPE_ARTS",
            CommandType.Quick => "FGOCORE-COMMAND_TYPE_QUICK",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return new LocString("cards", key);
    }
}
