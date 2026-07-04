using BaseLib.Extensions;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Powers.Forms;

/// <summary>Base de las formas de Oberon sobre el FormPower de FGOCore. Iconos en este mod
/// (espejo de TiamatFormPower).</summary>
public abstract class OberonFormPower : FormPower
{
    // Re-chequeo de la ulti auto-manifestada a inicio de cada turno (siempre hay UNA forma activa):
    // cubre la carga que quedo sentada en >=100 sin cruzar (Kaleidoscope/refunds) y la re-manifiesta
    // si la copia anterior se jugo/exhausteo. Idempotente (EnsureUltInHand no duplica).
    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side, System.Collections.Generic.IReadOnlyList<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants, MegaCrit.Sts2.Core.Combat.ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);
        if (side == Owner.Side)
        {
            await MainFile.EnsureUltInHand(Owner);
        }
    }

    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
