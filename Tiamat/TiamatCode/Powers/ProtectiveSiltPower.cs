using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Limo Protector — pasiva defensiva persistente: al inicio de TUS turnos ganás Bloqueo igual a las
/// acumulaciones (el lodo primordial se endurece solo). Análogo directo de Metallicize (poco común
/// vanilla, 3 bloqueo/turno). Rebalance 2026-08-15: era Baluarte por turno, o sea un Metallicize
/// estrictamente mejor cuyo piso retenido COMPONÍA combate entero (reportes de inmortalidad, ver
/// docs/REBALANCE-TIAMAT-ARTORIA.md); ahora es Bloqueo plano y la retención de Tiamat vuelve a
/// salir solo de las cartas de Baluarte jugadas. La concede la carta poco común
/// <c>ProtectiveSilt</c>. Apilable (Counter = Bloqueo por turno).
/// </summary>
public sealed class ProtectiveSiltPower : TiamatPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}
