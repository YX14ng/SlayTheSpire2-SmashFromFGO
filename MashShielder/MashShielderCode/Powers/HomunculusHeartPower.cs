using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Corazón del Homúnculo — whenever Mash changes form: draw cards and gain NP Charge.
/// Stacks scale the effect (1 stack: draw 2, NP +10; 2 stacks: draw 3, NP +15).
/// </summary>
public sealed class HomunculusHeartPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        Flash();
        if (choiceContext != null && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, 1 + Amount, Owner.Player);
        }
        await NpCharge.Gain(Owner, 5 + 5 * Amount, null);
    }
}
