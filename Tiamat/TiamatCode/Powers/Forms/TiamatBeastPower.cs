using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Tiamat.TiamatCode.Powers.Forms;

/// <summary>
/// Forma Dracónica (Bestia II) — la DEVORADORA. (1) Amplifica la Maldición que aplicás (+1,
/// ICurseAmplifier) y esparce Marea sola: al inicio de tu turno aplica +1 Maldición a un
/// enemigo. (2) El enjambre MUERDE DOS VECES (ISwarmBiteAmplifier). (3) Devorar +50%
/// (IDevourAmplifier). Es la forma clímax donde el enjambre ya criado se vuelve daño (se entra
/// por la ventana de NP "Génesis"). Rediseño 2026-06-13.
/// </summary>
public sealed class TiamatBeastPower : TiamatFormPower, ICurseAmplifier, ISwarmBiteAmplifier, IDevourAmplifier
{
    public override string FramesPath => $"{MainFile.ResPath}/character/tiamat_frames_beast.tres";

    public int ExtraCurse => 1;

    public int ExtraBites => 1; // el enjambre muerde dos veces (1 base + 1)

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        var enemy = Owner.CombatState.GetOpponentsOf(Owner).FirstOrDefault(e => !e.IsDead);
        if (enemy == null) return;
        Flash();
        await Curses.Apply(enemy, 1, Owner, null);
    }
}
