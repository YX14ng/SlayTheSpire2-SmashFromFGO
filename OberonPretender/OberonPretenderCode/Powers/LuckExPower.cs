using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Suerte EX (Luck EX) -- DESIGN-OBERON 6.4. Al inicio de tu turno: +<see cref="Stars"/>
/// Estrellas (la star gen 20,5% real de Oberon hecha motor). El up sube el rider (la carta fija el campo).
/// </summary>
public sealed class LuckExPower : OberonPower
{
    public int Stars = 20;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await CritStars.Gain(Owner, Stars, null);
    }
}
