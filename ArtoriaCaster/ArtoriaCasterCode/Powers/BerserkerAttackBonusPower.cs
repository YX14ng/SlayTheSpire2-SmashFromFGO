using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Base de los powers "+Amount de daño a tus Ataques mientras estés en forma Berserker (o Avalon)".
/// El cuerpo de <see cref="ModifyDamageAdditive"/> era IDÉNTICO en MidsummerMadnessPower y
/// SummerRivalryPower; vive una sola vez acá. Las dos siguen siendo powers distintos (entradas de
/// pool/loc/icono separadas, son stacks deliberados del mismo arquetipo) pero comparten el motor.
/// </summary>
public abstract class BerserkerAttackBonusPower : ArtoriaPower
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
