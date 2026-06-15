using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Base común de los powers "al inicio de tu turno: aplica Amount de Maldición a
/// TODOS los enemigos vivos". <see cref="FairyEyesPower"/> (妖精眼) e
/// <see cref="PerpetualWinterPower"/> (永恒之冬) eran funcionalmente idénticos —
/// el <c>AfterPlayerTurnStart</c> vive ahora una sola vez aquí. Las subclases
/// se mantienen como identidades separadas (loc/icono/ID propios); solo difieren
/// por su tipo (que resuelve su <c>CustomPackedIconPath</c> en <see cref="MorganPower"/>).
/// </summary>
public abstract class StartOfTurnCurseAllPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        foreach (var enemy in Owner.CombatState.GetOpponentsOf(Owner))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(enemy, Amount, Owner, null);
            }
        }
    }
}
