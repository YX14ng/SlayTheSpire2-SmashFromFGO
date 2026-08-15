using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Vigilia del Spriggan (斯普利坎的守夜) — re-efecto RE-POOL V2 (§5.3, cap conservador de P2 /
/// parche J1-17): al inicio de tus turnos ganás Bloqueo IGUAL a la Maldición del enemigo más
/// maldito, con tope = Amount (10 base, 14 mejorada). Conversión Maldición→defensa del arquetipo
/// B: el guardián de la corte vigila mientras el campo se pudre. Antes era Bloqueo fijo + NP.
/// </summary>
public sealed class SprigganVigilPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        if (Owner.CombatState is not { } combatState) return;

        var mostCursed = combatState.GetOpponentsOf(Owner)
            .Where(e => !e.IsDead)
            .Select(Curses.Of)
            .DefaultIfEmpty(0)
            .Max();
        var block = System.Math.Min(mostCursed, (int)Amount);
        if (block <= 0) return;

        Flash();
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);
    }
}
