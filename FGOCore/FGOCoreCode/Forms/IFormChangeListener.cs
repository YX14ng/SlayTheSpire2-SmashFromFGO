using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace FGOCore.FGOCoreCode.Forms;

/// <summary>
/// A power that reacts when its owner changes form (e.g. Mash's Homunculus Heart:
/// draw + NP on form change). Notified by <see cref="FormSwitch.Enter{T}"/> for
/// player-initiated switches only.
/// </summary>
public interface IFormChangeListener
{
    Task OnFormChanged(PlayerChoiceContext? choiceContext);
}
