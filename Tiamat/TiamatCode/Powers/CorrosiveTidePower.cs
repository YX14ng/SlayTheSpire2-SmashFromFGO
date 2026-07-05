using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Marea Corrosiva — pasiva persistente: al inicio de cada turno tuyo aplicás <see cref="CursePerStack"/>
/// de Maldición a TODOS los enemigos por acumulación (la marea de corrupción sube sola y siembra el campo
/// que el enjambre cosecha y que las cartas de Maldición cobran). La concede la carta común homónima.
/// Apilable (Counter): jugarla de nuevo suma otra acumulación.
/// </summary>
public sealed class CorrosiveTidePower : TiamatPower
{
    public const int CursePerStack = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        foreach (var enemy in Owner.CombatState.GetOpponentsOf(Owner).Where(e => !e.IsDead).ToList())
        {
            // applier null (audit 2026-07-05): el spread automatico NO pasa por ICurseAmplifier — con Owner,
            // la forma Bestia le sumaba +1 por enemigo por turno, contradiciendo el texto del power
            // (espejo del spread de TiamatBeastPower, que tambien pasa null).
            await Curses.Apply(enemy, CursePerStack * Amount, null, null);
        }
    }
}
