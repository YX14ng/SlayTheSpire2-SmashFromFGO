using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Corona del Relámpago (雷之冠, §5.3) — al inicio de cada turno ganás <see cref="StarsPerTurn"/>
/// Estrellas de Crítico (10, up 20). Per-turn ★, slot Angel. El valor por activación es campo
/// settable que fija la carta desde su DynamicVar (patrón WeightOfExpectationsPower); Amount es
/// solo el conteo de stacks (Counter, las copias apilan).
/// </summary>
public sealed class CrownOfLightningPower : MordredPower
{
    public int StarsPerTurn = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await CritStars.Gain(Owner, StarsPerTurn * (int)Amount, null);
    }
}
