using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Soberana de Dos Rostros (双面君主) — whenever you change form: draw 2 and NP +10.
/// Notified by FGOCore's FormSwitch via IFormChangeListener.
/// </summary>
public sealed class SovereignOfTwoFacesPower : MorganPower, IFormChangeListener
{
    public const int Draws = 2;
    public const int NpGain = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        Flash();
        if (choiceContext != null && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, Draws, Owner.Player);
        }
        await NpCharge.Gain(Owner, NpGain, null);
    }
}
