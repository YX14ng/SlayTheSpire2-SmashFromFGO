using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using FGOCore.FGOCoreCode.Stars;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Un Fragmento de 2030 (2030年的碎片) — al inicio de cada uno de tus turnos ganás
/// <see cref="Amount"/> Estrellas de Crítico (motor de generación pasiva). Counter:
/// stacks suman las estrellas por turno. Espejo del timing de <c>CursePower</c>
/// (<see cref="AfterSideTurnStart"/>) pero usando el helper compartido
/// <see cref="CritStars.Gain"/> para que respete el banco/auto-proc del owner.
/// Personal: no escala en multijugador.
/// </summary>
public sealed class Fragment2030Power : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.IsDead) return;
        Flash();
        await CritStars.Gain(Owner, (int)Amount, null);
    }
}
