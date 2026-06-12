using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Corte del Invierno+ (冬之宫廷+) — draw Amount additional card(s) each turn.
/// Parche P10 del rediseño v2: lo aplica SOLO la WinterCourt mejorada y devuelve
/// al pool el robo sostenido que el rediseño le quitó al WinterCourt original
/// (la velocidad de mazo es el combustible del ciclo NP/estrellas).
/// </summary>
public sealed class WinterCourtDrawPower : MorganPower
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
