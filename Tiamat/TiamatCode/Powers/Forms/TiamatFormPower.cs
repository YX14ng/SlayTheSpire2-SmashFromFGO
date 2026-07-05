using BaseLib.Extensions;
using TiamatBeast.TiamatCode.Extensions;

namespace TiamatBeast.TiamatCode.Powers.Forms;

/// <summary>Base de las formas de Tiamat sobre el FormPower de FGOCore. Íconos en este mod.</summary>
public abstract class TiamatFormPower : FormPower
{
    // Re-chequeo del Genesis a inicio de cada turno (audit 2026-07-05): cubre el medidor que quedo
    // sentado en >=100 al expirar la ventana Bestia sin jugar Pluma. Idempotente.
    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side, System.Collections.Generic.IReadOnlyList<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants, MegaCrit.Sts2.Core.Combat.ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);
        if (side == Owner.Side)
        {
            await MainFile.EnsureGenesisInHand(Owner);
        }
    }

    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
