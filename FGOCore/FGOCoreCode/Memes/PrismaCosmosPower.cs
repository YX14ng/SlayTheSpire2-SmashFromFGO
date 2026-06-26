using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using FGOCore.FGOCoreCode.Np;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Prisma Cosmos (棱镜星云) — al inicio de cada uno de tus turnos ganás
/// <see cref="Amount"/> de Carga NP (motor de generación pasiva, el CE de carga clásico).
/// Counter: stacks suman la carga por turno. Pasa por <see cref="NpCharge.Gain"/>, así que
/// respeta el tope dinámico del medidor, dispara GaugeFilled y se amplifica con Regla de Oro.
/// Personal: no escala en multijugador.
/// </summary>
public sealed class PrismaCosmosPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        Flash();
        await NpCharge.Gain(Owner, (int)Amount, null);
    }
}
