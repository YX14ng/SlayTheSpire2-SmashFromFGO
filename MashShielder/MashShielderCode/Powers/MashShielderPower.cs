using BaseLib.Abstracts;
using BaseLib.Extensions;
using MashShielder.MashShielderCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// This is the base class for your mod's powers, which is set up to load the power's images from your mod's resources.
/// When creating a power, right click the Powers folder and create a new file with the Custom Power template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
public abstract class MashShielderPower : CustomPowerModel
{
    //Loads from MashShielder/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    /// <summary>
    /// Whether this power is a buff or debuff.
    /// </summary>
    public abstract override PowerType Type { get; }
    
    /// <summary>
    /// How this power stacks if reapplied. Counter is the most common type, where applying the power again just
    /// adds to the amount. Single means the power does not stack, like Barricade. None functions identically to
    /// Single, but you're suggested to use Single as it is more explicit about how it will work.
    /// </summary>
    public abstract override PowerStackType StackType { get; }

    /// <summary>
    /// Helper local de modularización (AUDIT-2026-06-15): centraliza el patrón
    /// "resetear un contador/flag al inicio del turno DEL JUGADOR" que se repetía idéntico en
    /// varios powers (WallDoctrine, ConceptualAmmo, OrtinaxServos, HomunculusHeart).
    /// Las subclases sólo sobrescriben <see cref="OnPlayerTurnStartReset"/> con su reset; el
    /// guard <c>side == CombatSide.Player</c> y el <c>Task.CompletedTask</c> viven una sola vez.
    /// </summary>
    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) OnPlayerTurnStartReset();
        return Task.CompletedTask;
    }

    /// <summary>Se llama una vez al inicio de CADA turno del jugador. Por defecto no hace nada.</summary>
    protected virtual void OnPlayerTurnStartReset() { }
}