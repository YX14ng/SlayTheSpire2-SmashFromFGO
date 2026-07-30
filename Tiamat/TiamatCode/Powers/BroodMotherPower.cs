using FGOCore.FGOCoreCode.Lahmu;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Cría de la Madre — pasiva persistente del combate: al inicio de TUS turnos parís 1 Laḫmu por
/// acumulación (la Madre desova sola). El motor de población que mantiene el enjambre lleno turno a
/// turno, así la mordida de Lily y la cosecha Bestia nunca se quedan sin cría. La concede la carta
/// rara <c>BroodNest</c>. Apilable (Counter).
/// </summary>
public sealed class BroodMotherPower : TiamatPower
{
    public const int LahmuPerStack = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<LahmuSwarmPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await Lahmu.Spawn(choiceContext, Owner, LahmuPerStack * Amount, null);
    }
}
