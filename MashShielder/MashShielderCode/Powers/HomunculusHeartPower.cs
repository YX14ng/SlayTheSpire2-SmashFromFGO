using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Corazón del Homúnculo — whenever Mash changes form: draw cards and gain NP Charge.
/// Stacks scale the effect (1 stack: draw 2, NP +10; 2 stacks: draw 3, NP +20).
/// Parche P2 del rediseño v2: máximo 2 procs por turno (corta el loop de robo
/// FormDrill × HomunculusHeart a 0E; el ping-pong de formas sigue, regla §5).
/// Notified by FGOCore's FormSwitch via IFormChangeListener.
/// </summary>
public sealed class HomunculusHeartPower : MashShielderPower, IFormChangeListener
{
    public const int MaxProcsPerTurn = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int _procsThisTurn;

    protected override void OnPlayerTurnStartReset() => _procsThisTurn = 0;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_procsThisTurn >= MaxProcsPerTurn) return;
        _procsThisTurn++;
        Flash();
        if (choiceContext != null && Owner.Player != null)
        {
            // BUGFIX (soft-lock): el cambio de forma lo dispara una carta a MITAD de su resolución.
            // Si este robo RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
            // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
            // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
            var inDeck = Owner.Player.PlayerCombatState.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            var toDraw = System.Math.Min(1 + Amount, inDeck);
            if (toDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, toDraw, Owner.Player);
            }
        }
        await NpCharge.Gain(Owner, 10 * Amount, null);
    }
}
