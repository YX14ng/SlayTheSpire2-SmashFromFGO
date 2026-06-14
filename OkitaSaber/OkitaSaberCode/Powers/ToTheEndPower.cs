using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using OkitaSaber.OkitaSaberCode.Cards;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Hasta el Final (直到最后) — ESTE TURNO tus *RÁFAGAS no cuestan *Aliento; en su lugar pagás
/// <see cref="HpPerBreathPoint"/> HP (Unblockable/Unpowered) por cada punto que hubieras pagado
/// (DESIGN-OKITA §5.4: 2 HP/punto; up 1). El plan nunca muere, el cuerpo paga. Lo aplica la carta
/// homónima (0⚡, Exhaust). Se auto-remueve al terminar tu turno (patrón ExposedBackPower). Single.
///
/// El cobro de HP lo hace <see cref="Rafaga.Pay"/> al leer este <see cref="IRafagaCostModifier"/>.
/// </summary>
public sealed class ToTheEndPower : OkitaPower, IRafagaCostModifier
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>HP a pagar por punto de Aliento ahorrado (2 base; la carta lo baja a 1 con la mejora).</summary>
    public int HpCostPerPoint = 2;

    public bool WaivesBreathCost => true;

    public int HpPerBreathPoint => HpCostPerPoint;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}
