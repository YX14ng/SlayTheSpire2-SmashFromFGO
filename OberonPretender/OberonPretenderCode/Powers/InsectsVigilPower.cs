using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Vigilia del Insecto (Insect's Vigil) -- DESIGN-OBERON 6.3. Al inicio de tu turno:
/// +<see cref="Charge"/> NP (up: ademas +<see cref="Stars"/> Estrellas). Motor de bateria pasivo.
/// </summary>
public sealed class InsectsVigilPower : OberonPower
{
    public int Charge = 10;
    public int Stars; // 0 base; el up sube a 5

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await NpCharge.Gain(Owner, Charge, null);
        if (Stars > 0) await CritStars.Gain(Owner, Stars, null);
    }
}
