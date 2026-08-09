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
    public const int StarsGain = 10;
    public const int NpGain = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>Cartas robadas por cambio de forma (1 base, 2 con la carta mejorada).</summary>
    public int Draws { get; set; } = 1;

    /// <summary>
    /// Robos que el mazo no pudo cubrir durante la resolución de la carta que cambió la forma.
    /// Se saldan en <see cref="AfterCardPlayed"/>, cuando el reshuffle ya es seguro.
    /// </summary>
    private int _pendingDraws;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        Flash();
        var player = Owner.Player;
        if (choiceContext != null && player?.PlayerCombatState is { } playerCombatState)
        {
            // BUGFIX (soft-lock): el cambio de forma lo dispara una carta (p.ej. SummerOutburst) a
            // MITAD de su resolución. Si este robo RESHUFFLEA (mazo vacío), el reshuffle corrompe
            // el estado de la carta en curso ("must be added to a CombatState", carta colgada).
            // Por eso acá robamos SOLO lo que hay en el mazo; el resto queda pendiente y se roba
            // en AfterCardPlayed, cuando la carta terminó de resolverse (patrón Driftwood vanilla:
            // esa reliquia roba en AfterCardPlayed y el reshuffle ahí es seguro).
            var inDeck = playerCombatState.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            var toDraw = System.Math.Min(Draws, inDeck);
            if (toDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, toDraw, player);
            }
            _pendingDraws += Draws - toDraw;
        }
        var context = choiceContext ?? new BlockingPlayerChoiceContext();
        await Stars.Gain(context, Owner, StarsGain, null);
        await NpCharge.Gain(context, Owner, NpGain, null);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        if (_pendingDraws <= 0) return;

        var pending = _pendingDraws;
        _pendingDraws = 0;
        var player = Owner.Player;
        if (player?.PlayerCombatState != null)
        {
            await CardPileCmd.Draw(choiceContext, pending, player);
        }
    }
}
