using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Ambición del Trono (王座野望, §5.3) — al inicio de cada turno ganás <see cref="NpPerTurn"/>
/// de Carga NP (10, up 15). Per-turn NP, slot Bendición de Avalon. El valor por activación es
/// campo settable que fija la carta desde su DynamicVar; Amount es el conteo de stacks (Counter).
/// </summary>
public sealed class AmbitionForThronePower : MordredPower
{
    public int NpPerTurn = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await NpCharge.Gain(Owner, NpPerTurn * (int)Amount, null);
    }
}
