using System.Collections.Generic;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

/// <summary>
/// Base for Castoria's forms on top of FGOCore's generic FormPower.
/// Icons live in ArtoriaCaster's resources, not FGOCore's.
/// </summary>
public abstract class ArtoriaFormPower : FormPower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    // A inicio de cada turno re-chequea la ulti auto-manifestada (siempre hay UNA forma activa, así que
    // este hook corre siempre). Cubre el caso del Kaleidoscope/carga inicial que te deja en >=100 sin
    // pasar por el cruce de GaugeFilled → si no, quedabas atascado en >=100 sin la ulti en mano.
    // Idempotente (EnsureUltInHand no duplica si ya está en mano).
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);
        if (side == Owner.Side)
        {
            await MainFile.EnsureUltInHand(Owner);
        }
    }
}
