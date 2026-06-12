using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// El Milagro de Fou — the first time this combat that damage would kill you,
/// you survive at this power's Amount HP, gain a burst of Block and NP Charge
/// (rediseño v2: +50 NP — el milagro arma la ult de la remontada).
/// </summary>
public sealed class FouMiraclePower : MashShielderPower
{
    public const int RescueBlock = 25;

    public const int RescueNpCharge = 50;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _triggered;

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_triggered || target != Owner) return amount;

        var hp = Owner.CurrentHp;
        if (hp <= 0 || amount < hp) return amount;

        _triggered = true;
        var floor = Math.Min(Amount, hp);
        return hp - floor;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!_triggered || target != Owner) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, RescueBlock, ValueProp.Unpowered, null);
        await NpCharge.Gain(Owner, RescueNpCharge, null);
        await PowerCmd.Remove(this);
    }
}
