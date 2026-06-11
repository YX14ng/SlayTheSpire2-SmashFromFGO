using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>Corte del Invierno (冬之宫廷) — draw Amount additional cards each turn.</summary>
public sealed class WinterCourtPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // OJO: ModifyHandDraw es ABSOLUTO (default devuelve el input).
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player) return count;
        return count + Amount;
    }
}
