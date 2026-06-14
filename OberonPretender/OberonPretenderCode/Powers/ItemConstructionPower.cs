using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Construccion de Items A+ (Item Construction A+) -- DESIGN-OBERON 6.3. Marcador en el
/// jugador: tus Debil y Vulnerable aplican +1 stack (el rocio tricolor con que maldijo a Titania).
/// Las cartas que aplican Debil/Vulnerable leen <c>HasPower<ItemConstructionPower>()</c> y suman
/// <see cref="ExtraStacks"/> al aplicar (patron ICurseAmplifier, lado-carta). El up agrega +5 NP por
/// aplicacion (lo gestiona la carta via el flag <see cref="RefundsCharge"/>).
/// </summary>
public sealed class ItemConstructionPower : OberonPower
{
    public const int ExtraStacks = 1;
    public const int ChargePerApply = 5;

    public bool RefundsCharge; // lo fija la carta desde IsUpgraded

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>Stacks extra de Debil/Vulnerable que aplica este dueno (lectura pura para las cartas).</summary>
    public static int ExtraDebuffStacks(Creature creature) =>
        creature.HasPower<ItemConstructionPower>() ? ExtraStacks : 0;
}
