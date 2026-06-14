using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TiamatBeast.TiamatCode.Powers.Forms;

/// <summary>
/// Femme Fatale (humanoide) — la CRIADORA. Forma inicial de Tiamat. Al inicio de tu turno:
/// +1 Crianza (la madre amamanta). Es donde construís el enjambre barato; el daño grande
/// llega cosechando en la forma Bestia.
/// </summary>
public sealed class TiamatFemmeFatalePower : TiamatFormPower
{
    public override string FramesPath => $"{MainFile.ResPath}/character/tiamat_frames_human.tres";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await Lahmu.Feed(Owner, 1, null);
    }
}
