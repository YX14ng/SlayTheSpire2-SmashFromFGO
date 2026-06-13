using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.DragonScales;

/// <summary>
/// Fan-out for the "a hit pierced the Dragon Scales" event. Mirrors
/// <c>FormSwitch</c>'s listener broadcast: the relic that owns the pierce (Linden Leaf)
/// calls <see cref="Broadcast"/> ONCE per turn, on the real damage path, after it has
/// consumed its per-turn pierce allowance. Every <see cref="IDragonScalePierceListener"/>
/// power on the owner reacts (e.g. the Linden Scar card-power: +1 scale, +NP).
/// FGOCore never references any character mod — listeners are discovered by interface.
/// </summary>
public static class DragonScalesPierce
{
    public static async Task Broadcast(Creature owner, PlayerChoiceContext choiceContext)
    {
        foreach (var listener in owner.GetPowerInstances<PowerModel>().OfType<IDragonScalePierceListener>().ToList())
        {
            await listener.OnScalesPierced(choiceContext);
        }
    }
}
