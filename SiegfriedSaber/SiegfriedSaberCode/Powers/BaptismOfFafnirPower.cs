using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Bautismo de Fafnir — power-listener oculto que aplica la reliquia FafnirHeartblood al iniciar combate.
/// NECESARIO porque DragonScalesPierce.Broadcast sólo descubre POWERS (no relics): cuando un golpe ATRAVIESA
/// la Sangre de Dragón (≤1/turno, estructural), la herida se bautiza: +<see cref="Amount"/> Carga NP.
/// Reusa la interfaz EXISTENTE IDragonScalePierceListener (sin tocar FGOCore). No solapa con el +5 NP de la
/// Hoja de Tilo (ese es en golpe REDUCIDO; éste, en el pierce — nunca el mismo golpe).
/// </summary>
public sealed class BaptismOfFafnirPower : SiegfriedPower, IDragonScalePierceListener
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnScalesPierced(PlayerChoiceContext choiceContext)
    {
        Flash();
        await NpCharge.Gain(Owner, Amount, null);
    }
}
