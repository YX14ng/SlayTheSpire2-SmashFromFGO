using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Euforia Nocturna (Nocturnal Euphoria) -- premia la SOLVENCIA (DESIGN-OBERON 6.3, lore S1).
/// Al final de tu turno, si NO te queda Deuda impaga: +<see cref="Charge"/> NP y +<see cref="Stars"/>
/// Estrellas. Corre DESPUES del cobro de la Deuda (el cobro va en DebtPower.BeforeTurnEnd; esto en
/// AfterTurnEnd -> la billetera ya saldo). El up sube ambos riders (la carta fija los campos).
/// </summary>
public sealed class NocturnalEuphoriaPower : OberonPower
{
    public int Charge = 10;
    public int Stars = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        if (DebtPower.Of(Owner) > 0) return; // solo si estas al dia
        Flash();
        await NpCharge.Gain(Owner, Charge, null);
        await CritStars.Gain(Owner, Stars, null);
    }
}
