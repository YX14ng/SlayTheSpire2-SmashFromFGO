using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
            // BUGFIX (soft-lock): el cambio de forma lo dispara una carta (p.ej. SummerOutburst) a
            // MITAD de su resolución. Si este robo RESHUFFLEA (mazo vacío), reshufflea el descarte
            // -que en v0.107.1 contiene la carta en curso- y corrompe su estado: al descartarla el
            // juego tira "must be added to a CombatState" y la carta queda colgada mid-screen.
            // Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle de la carta jugada).
            var inDeck = Owner.Player.PlayerCombatState.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            var toDraw = System.Math.Min(Draws, inDeck);
            if (toDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, toDraw, Owner.Player);
            }
        }
        await Stars.Gain(Owner, StarsGain, null);
        await NpCharge.Gain(Owner, NpGain, null);
    }
}
