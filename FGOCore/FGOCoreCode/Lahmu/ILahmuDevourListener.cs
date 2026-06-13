using MegaCrit.Sts2.Core.Entities.Creatures;

namespace FGOCore.FGOCoreCode.Lahmu;

/// <summary>
/// La reliquia/power reacciona cuando su dueña DEVORA Laḫmu (sacrifica larvas del enjambre).
/// Lo dispara <see cref="Lahmu.Devour"/> tras consumir las larvas, con cuántas se devoraron.
/// (Espejo de ICurseAmplifier/IGutsFloorBooster: gancho en FGOCore para que el mod no tenga
/// que parchear el helper compartido.) Usado por "Lágrimas de la Madre" (curar al devorar).
/// </summary>
public interface ILahmuDevourListener
{
    Task OnLahmuDevoured(Creature devourer, int eaten);
}
