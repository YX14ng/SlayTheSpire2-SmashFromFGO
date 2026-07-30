using System.Collections.Concurrent;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace FGOCore.FGOCoreCode.Attributes;

/// <summary>Atributo exclusivo de una criatura en el sistema FGO.</summary>
public enum FgoAttribute
{
    Man,
    Earth,
    Heaven,
    Star,
    Beast
}

/// <summary>Origen diagnosticable de una resolución de atributo.</summary>
public enum FgoAttributeSource
{
    None,
    EncounterDefault,
    ExplicitOverride
}

public readonly record struct FgoAttributeResolution(
    FgoAttribute? Attribute,
    FgoAttributeSource Source);

/// <summary>
/// Fuente única de atributos FGO. No usa powers: un atributo es metadata inmutable de combate,
/// no un Buff que pueda limpiarse, duplicarse o amplificarse.
/// </summary>
public static class FgoAttributes
{
    private static readonly ConcurrentDictionary<ModelId, FgoAttribute> Overrides = new();

    /// <summary>Registra o reemplaza el atributo explícito de un modelo de criatura/personaje.</summary>
    public static void RegisterOverride(ModelId modelId, FgoAttribute attribute) =>
        Overrides[modelId] = attribute;

    /// <summary>Quita un override registrado; vuelve a aplicar la convención del encuentro.</summary>
    public static bool RemoveOverride(ModelId modelId) => Overrides.TryRemove(modelId, out _);

    public static FgoAttribute? Of(Creature creature) => Resolve(creature).Attribute;

    public static bool Is(Creature creature, FgoAttribute attribute) => Of(creature) == attribute;

    /// <summary>
    /// Resuelve primero overrides por ModelId. Sin override, Monster/Elite/Boss mapean a
    /// Man/Earth/Heaven. Event y cualquier sala ambigua quedan sin atributo.
    /// </summary>
    public static FgoAttributeResolution Resolve(Creature creature)
    {
        if (Overrides.TryGetValue(creature.ModelId, out var explicitAttribute))
        {
            return new FgoAttributeResolution(explicitAttribute, FgoAttributeSource.ExplicitOverride);
        }

        var roomType = creature.CombatState?.Encounter?.RoomType;
        var attribute = roomType switch
        {
            RoomType.Monster => FgoAttribute.Man,
            RoomType.Elite => FgoAttribute.Earth,
            RoomType.Boss => FgoAttribute.Heaven,
            _ => (FgoAttribute?)null
        };

        return new FgoAttributeResolution(
            attribute,
            attribute.HasValue ? FgoAttributeSource.EncounterDefault : FgoAttributeSource.None);
    }
}
