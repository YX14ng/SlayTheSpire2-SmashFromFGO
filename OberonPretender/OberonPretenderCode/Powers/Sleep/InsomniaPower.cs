using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OberonPretender.OberonPretenderCode.Powers.Sleep;

/// <summary>
/// Insomnio (Insomnia) -- el candado anti-stunlock (DESIGN-OBERON 3.d). Tras despertar de
/// un Sueno, el enemigo gana Insomnio 2: mientras tenga Insomnio NO puede volver a dormirse
/// (<see cref="Sleep.TryApply"/> lo respeta). Cuenta atras 1 al inicio del turno del DUENO (el
/// enemigo) -- patron decremento de CursePower/AsleepPower. Contador para que dos despertares no
/// pisen su ventana.
/// </summary>
public sealed class InsomniaPower : OberonPower
{
    public const int Duration = 2;

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        await PowerCmd.Decrement(this);
    }
}
