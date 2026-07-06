using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Limo Protector — pasiva defensiva persistente: al inicio de TUS turnos ganás Baluarte igual a las
/// acumulaciones (el lodo primordial se endurece solo). Análogo directo de Metallicize (poco común
/// vanilla, 3 bloqueo/turno) con la identidad Tiamat: es Baluarte (se RETIENE entre turnos, capado por
/// <see cref="BlockRetention"/>), así alimenta el plan de tortuga de Lily sin tocar el enjambre.
/// La concede la carta poco común <c>ProtectiveSilt</c>. Apilable (Counter = Baluarte por turno).
/// </summary>
public sealed class ProtectiveSiltPower : TiamatPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BulwarkPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await BlockRetention.GainBulwarkBlock(null, Owner, Amount, choiceContext: choiceContext);
    }
}
