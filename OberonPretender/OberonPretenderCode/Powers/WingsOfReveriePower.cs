using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using OberonPretender.OberonPretenderCode.Cards;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Alas del Ensueno (Wings of Reverie) -- DESIGN-OBERON 6.4. Cuando jugas una carta NP
/// (<see cref="IOberonNpCard"/>): +<see cref="Stars"/> Estrellas y roba 1 (los finishers realimentan
/// la economia). El up sube las Estrellas (la carta fija el campo).
/// </summary>
public sealed class WingsOfReveriePower : OberonPower
{
    public int Stars = 20;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not IOberonNpCard || cardPlay.Card.Owner?.Creature != Owner || Owner.Player == null) return;
        Flash();
        await CritStars.Gain(Owner, Stars, null);
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), 1, Owner.Player);
    }
}
