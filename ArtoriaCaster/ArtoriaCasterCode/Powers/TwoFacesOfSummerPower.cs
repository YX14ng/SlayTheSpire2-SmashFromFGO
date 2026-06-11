using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Dos Caras del Verano — cada vez que cambiás de forma: robás <see cref="Draws"/>,
/// +1★ y Carga NP +5. Notificado por el FormSwitch de FGOCore vía IFormChangeListener
/// (patrón Soberana de Dos Rostros de Morgan). El robo solo ocurre en cambios
/// iniciados por el jugador (choiceContext != null).
/// </summary>
public sealed class TwoFacesOfSummerPower : ArtoriaPower, IFormChangeListener
{
    public const int StarsGain = 1;
    public const int NpGain = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>Cartas robadas por cambio de forma (1 base, 2 con la carta mejorada).</summary>
    public int Draws { get; set; } = 1;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        Flash();
        if (choiceContext != null && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, Draws, Owner.Player);
        }
        await Stars.Gain(Owner, StarsGain, null);
        await NpCharge.Gain(Owner, NpGain, null);
    }
}
