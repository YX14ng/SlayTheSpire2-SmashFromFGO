using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Manipulación de la Espada Sagrada A (聖剣操作) — al inicio de tu turno: ganás
/// Amount Estrella(s) Crítica(s) (patrón RainWitchFormPower de Morgan).
/// </summary>
public sealed class SacredSwordControlPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await Stars.Gain(Owner, Amount, null);
    }
}
