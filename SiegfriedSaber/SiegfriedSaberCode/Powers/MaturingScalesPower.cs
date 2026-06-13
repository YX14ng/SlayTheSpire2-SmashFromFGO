using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Escamas que Maduran (渐厚之鳞) — al inicio de CADA uno de tus turnos, +<see cref="Amount"/> Sangre de
/// Dragón. Espejo DEFENSIVO de DemonFormPower (firma AfterSideTurnStart 1:1). Reusa DragonScalesPower.
/// Capa personal: no escala en multijugador.
/// </summary>
public sealed class MaturingScalesPower : SiegfriedPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
        {
            Flash();
            await PowerCmd.Apply<DragonScalesPower>(Owner, base.Amount, Owner, null);
        }
    }
}
