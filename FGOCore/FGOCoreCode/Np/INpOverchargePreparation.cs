using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace FGOCore.FGOCoreCode.Np;

/// <summary>
/// Contrato para preparaciones externas que elevan el tier efectivo del próximo NP.
/// Vive en FGOCore para que una preparación aplicada por un personaje funcione en cualquier aliado.
/// </summary>
public interface INpOverchargePreparation
{
    int ExtraTier { get; }
    Task ConsumeOverchargePreparation(PlayerChoiceContext context);
}
