using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Visperas del Fin (Vespers of the End) -- DESIGN-OBERON 6.4. SOLO en VORTIGERN: tus
/// Ataques hacen +<see cref="Bonus"/> (apila con el +3 propio de la forma). Fuera de Vortigern no hace
/// nada (el rezo del fin solo lo oye el insecto). El up sube el bonus (la carta fija el campo).
/// </summary>
public sealed class VespersOfTheEndPower : OberonPower
{
    public int Bonus = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || !Owner.HasPower<VortigernPower>()) return 0m;
        return Bonus;
    }
}
