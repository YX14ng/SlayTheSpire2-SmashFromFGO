using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Soberana de Dos Rostros (双面君主) — rediseño 2026-06-15 (swap Estrellas→Maldición): whenever
/// you change form: draw 2 and NP +10 (premia la danza Caster↔Berserker, el corazón del motor).
/// Notified by FGOCore's FormSwitch via IFormChangeListener.
/// </summary>
public sealed class SovereignOfTwoFacesPower : MorganPower, IFormChangeListener
{
    public const int Draws = 2;
    public const int NpGain = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        Flash();
        if (choiceContext != null && Owner.Player != null)
        {
            // BUGFIX (soft-lock): el cambio de forma lo dispara una carta a MITAD de su resolución.
            // Si este robo RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
            // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
            // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
            var inDeck = Owner.Player.PlayerCombatState.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            var toDraw = System.Math.Min(Draws, inDeck);
            if (toDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, toDraw, Owner.Player);
            }
        }
        await NpCharge.Gain(Owner, NpGain, null);
    }
}
