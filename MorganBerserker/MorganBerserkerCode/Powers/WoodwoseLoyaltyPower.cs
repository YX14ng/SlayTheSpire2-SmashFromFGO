using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Lealtad de Woodwose (伍德沃斯的忠诚) — RE-POOL V2 [NUEVA] (injerto de P1, parche J1-15):
/// retención de UN turno. La carta lo aplica con Amount = su Bloqueo cuando estás en Reina
/// Hada/Invierno; al inicio de tu próximo turno el Bloqueo se conserva hasta Amount (vía
/// <see cref="IBlockRetentionSource"/>/<see cref="BlockRetention"/>, contrato completo: también
/// respondemos <c>ShouldClearBlock</c> + <c>AfterPreventingBlockClear</c> → Enforce, patrón
/// DragonScaleAegis) y el power se retira — no compone entre turnos (la lección del Limo de
/// Tiamat). Tapa el hueco de ACAMPAR en la forma detonadora sin darle Baluarte permanente.
/// </summary>
public sealed class WoodwoseLoyaltyPower : MorganPower, IBlockRetentionSource
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public decimal RetentionCap(Creature creature) => creature == Owner ? Amount : 0m;

    public override bool ShouldClearBlock(Creature creature) => creature != Owner || Amount <= 0;

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != Owner) return;
        await BlockRetention.Enforce(creature);
        Flash();
    }

    public override async Task AfterBlockCleared(Creature creature)
    {
        // La retención ya ocurrió (o el clear la resolvió otra fuente): la lealtad dura UN turno.
        if (creature != Owner) return;
        await PowerCmd.Remove(this);
    }
}
