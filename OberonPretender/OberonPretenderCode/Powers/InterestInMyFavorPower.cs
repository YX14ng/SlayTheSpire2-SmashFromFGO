using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using OberonPretender.OberonPretenderCode.Cards;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Interes a Mi Favor (Interest in My Favor) -- DESIGN-OBERON 6.3. Cuando jugas una carta
/// de Prestamo (<see cref="ILoanCard"/>): +<see cref="Charge"/> NP (up: ademas +<see cref="Stars"/>
/// Estrellas). El motor del arquetipo Banca: convierte el acto de pedir prestado en bateria extra.
/// </summary>
public sealed class InterestInMyFavorPower : OberonPower
{
    public int Charge = 10;
    public int Stars; // 0 base; el up de la carta lo sube a 10

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not ILoanCard || cardPlay.Card.Owner?.Creature != Owner) return;
        Flash();
        await NpCharge.Gain(Owner, Charge, null);
        if (Stars > 0) await CritStars.Gain(Owner, Stars, null);
    }
}
