using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Locura de Pleno Verano (夏の狂化) — en forma Berserker (o Avalon) tus Ataques
/// hacen +Amount de daño.
/// </summary>
public sealed class MidsummerMadnessPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // ModifyDamageAdditive es DELTA (default 0).
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        if (!Owner.HasPower<SummerBerserkerFormPower>() && !Owner.HasPower<AvalonFormPower>()) return 0m;
        return Amount;
    }
}
