using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// Fragmento de la Mesa Redonda — starter relic: at the end of your turn, retain up to 10 Block.
/// </summary>
public sealed class RoundTableFragment : MashShielderRelic, IBlockRetentionSource
{
    public const int MaxRetainedBlock = 10;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(MaxRetainedBlock, ValueProp.Unpowered)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public decimal RetentionCap(Creature creature) =>
        Owner.GetRelic<LordCamelotRestored>() != null ? 0m : MaxRetainedBlock;

    public override async Task BeforeCombatStartLate()
    {
        await Powers.Forms.Forms.Enter<Powers.Forms.ShielderFormPower>(null, Owner.Creature, null);
    }

    public override bool ShouldClearBlock(Creature creature) => creature != Owner.Creature;

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != Owner.Creature) return;

        if (creature.Block == 0) return;
        await BlockRetention.Enforce(creature);
        Flash();
    }
}
