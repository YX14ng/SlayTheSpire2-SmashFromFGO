using MegaCrit.Sts2.Core.Entities.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Instinto de la Espada — tus CRÍTICOS consumen 1★ menos (mínimo 1, ya lo
/// garantiza <see cref="Stars.DiscountedCost"/> vía <see cref="ICritDiscount"/>).
/// </summary>
public sealed class SwordInstinctPower : ArtoriaPower, ICritDiscount
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public int CritCostReduction => 1;
}
