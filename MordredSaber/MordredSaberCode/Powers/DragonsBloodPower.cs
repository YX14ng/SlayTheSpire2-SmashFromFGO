using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Sangre de Dragón (龙之血, §5.2) — la vitalidad de homúnculo dracónico (trait Dragon). Al inicio
/// de cada turno ganás <see cref="NpPerTurn"/> de Carga NP (5) y <see cref="BlockPerTurn"/> de
/// Bloqueo (3, up 5). Engorda los hilos existentes (medidor + tanqueo) sin keyword nueva. Los
/// valores por activación son campos settables que fija la carta desde sus DynamicVars; Amount es
/// el conteo de stacks (Counter, las copias apilan). Patrón AmbitionForThronePower/MemoryOfTrifasPower.
/// </summary>
public sealed class DragonsBloodPower : MordredPower
{
    public int NpPerTurn = 5;
    public int BlockPerTurn = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await NpCharge.Gain(Owner, NpPerTurn * (int)Amount, null);
        await CreatureCmd.GainBlock(Owner, BlockPerTurn * (int)Amount, ValueProp.Unpowered, null);
    }
}
