using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Amenaza para la Humanidad (Threat to Humanity) -- el rasgo Beast de Oberon
/// (DESIGN-OBERON 6.3). Cuando un enemigo MUERE por tu mano: +<see cref="Charge"/> NP y
/// +<see cref="Stars"/> Estrellas. Lectura por <c>result.WasTargetKilled</c> con dealer == Owner.
/// </summary>
public sealed class ThreatToHumanityPower : OberonPower
{
    public int Charge = 20;
    public int Stars = 20;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !result.WasTargetKilled) return;
        Flash();
        await NpCharge.Gain(Owner, Charge, null);
        await CritStars.Gain(Owner, Stars, null);
    }
}
