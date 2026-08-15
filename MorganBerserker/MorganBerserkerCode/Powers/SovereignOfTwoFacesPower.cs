using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Soberana de Dos Rostros (双面君主) — rediseño 2026-06-15 (swap Estrellas→Maldición): whenever
/// you change form: draw 2 and NP +10 (premia la danza Caster↔Berserker, el corazón del motor).
/// RE-POOL V2 (J1-9/J2-9/J3-7, el cap más estricto del panel): máximo Amount veces por turno
/// (2 base, 3 mejorada) — sin el cap, el toggle común nuevo la volvía un motor de robo sin freno.
/// Contador en bits 7-8 del estado de turno. Notified by FGOCore's FormSwitch via IFormChangeListener.
/// </summary>
public sealed class SovereignOfTwoFacesPower : MorganPower, IFormChangeListener
{
    public const int Draws = 2;
    public const int NpGain = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        var used = FgoCombatState.GetTurn(Owner, 7, 2);
        if (used >= (int)Amount) return;
        await FgoCombatState.SetTurn(
            choiceContext ?? new BlockingPlayerChoiceContext(), Owner, 7, used + 1, null, 2);
        Flash();
        var player = Owner.Player;
        if (choiceContext != null && player?.PlayerCombatState is { } playerCombatState)
        {
            // BUGFIX (soft-lock): el cambio de forma lo dispara una carta a MITAD de su resolución.
            // Si este robo RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
            // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
            // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
            var inDeck = playerCombatState.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            var toDraw = System.Math.Min(Draws, inDeck);
            if (toDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, toDraw, player);
            }
        }
        await NpCharge.Gain(choiceContext ?? new BlockingPlayerChoiceContext(), Owner, NpGain, null);
    }
}
